using System;
using Application.DTO.PermissionDTO;
using Application.DTO.RoleDTO;
using Application.Repositories;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]

    public class RoleController : ControllerBase
	{
        private readonly IRoleRepository _repository;

        private readonly IPermissionRepository _permissionRep;

        private readonly IValidator<Role> _validator;

        private readonly IMapper _mapper;  


        public RoleController(IRoleRepository repository, IPermissionRepository permissionRepository, IValidator<Role> validator, IMapper mapper)
        {
            _repository = repository;
            _permissionRep = permissionRepository;
            _validator = validator;
            _mapper = mapper;
        }


        [HttpGet("[action]")]
        [Authorize(Roles = "GetAllRolesAsync")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            return Ok(_repository.GetAsync(x => true));
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "GetRoleById")]
        public async Task<IActionResult> GetRoleById([FromQuery] int Id)
        {
            Role role = await _repository.GetByIdAsync(Id);
            if (role == null)
            {
                return NotFound("Role from this id not found!!!");
            }
            return Ok(role);
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO roleCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role role = _mapper.Map<Role>(roleCreateDTO);
            List<Permission> permissions = new List<Permission>();
            for(int i = 0; i < role.Permissions.Count;i++)
            {
                Permission permission = role.Permissions.ToArray()[i];

                permission = await _permissionRep.GetByIdAsync(permission.PermissionId);
                if(permission == null)
                {
                    return NotFound($"Permission Not Found");
                }
                permissions.Add(permission);
            }
            role.Permissions = permissions;
            role = await _repository.AddAsync(role);
            if (role == null) return BadRequest(ModelState);

            RoleGetDTO roleGet = _mapper.Map<RoleGetDTO>(role);
            return Ok(roleGet);

        }

        [HttpPut("[action]")]
        [Authorize(Roles = "UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDTO roleUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            Role role = _mapper.Map<Role>(roleUpdateDTO);

            role = await _repository.UpdateAsync(role);
            if (role == null) return BadRequest(ModelState);

            RoleGetDTO roleGet = _mapper.Map<RoleGetDTO>(role);
            return Ok(roleGet);

        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromQuery] int Id)
        {
            bool isDelete = await _repository.DeleteAsync(Id);
            return isDelete ? Ok("Deleted successfully")
                : BadRequest("Delete operation failed");
        }
    }
}

