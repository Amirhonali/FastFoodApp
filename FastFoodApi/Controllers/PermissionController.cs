using System;
using Application.DTO.PermissionDTO;
using Application.DTO.ProductDTO;
using Application.Repositories;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]

    public class PermessionController : ControllerBase
    {
        private readonly IPermissionRepository _repository;

        private readonly IValidator<Permission> _validator;


        public PermessionController(IPermissionRepository repository, IValidator<Permission> validator)
        {
            _repository = repository;
            _validator = validator;
        }


        [HttpGet("[action]")]
        [Authorize(Roles = "GetAllPermission")]
        public async Task<IActionResult> GetAllPermissionsAsync()
        {
            return Ok(_repository.GetAsync(x => true));
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetPermissionById")]
        public async Task<IActionResult> GetPermissionById([FromQuery] int Id)
        {
            Permission permission = await _repository.GetByIdAsync(Id);
            if (permission == null)
            {
                return NotFound("Permission from this id not found!!!");
            }
            return Ok(permission);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "CreatePermission")]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateDTO permissionCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var permission = new Permission
            {
                PermissionName = permissionCreateDTO.PermissionName,
            };

            var validationResult = _validator.Validate(permission);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdPermission = await _repository.AddAsync(permission);
            return Ok(createdPermission);
        }

        [HttpPut("[action]")]
        [Authorize(Roles = "UpdatePermission")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionUpdateDTO permissionUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hasProd = await _repository.GetByIdAsync(id);
            if (hasProd == null)
            {
                return NotFound("Permission not found!!!");
            }

            hasProd.PermissionName = permissionUpdateDTO.PermissionName;

            var validationResult = _validator.Validate(hasProd);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatePermission = await _repository.UpdateAsync(hasProd);
            return Ok(updatePermission);
        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeletePermission")]
        public async Task<IActionResult> DeletePermission([FromQuery] int Id)
        {
            var existingProduct = await _repository.GetByIdAsync(Id);
            if (existingProduct == null)
            {
                return NotFound("Permission not found.");
            }

            await _repository.DeleteAsync(Id);
            return Ok($"Permission with ID {Id} has been deleted successfully.");
        }
    }
}

