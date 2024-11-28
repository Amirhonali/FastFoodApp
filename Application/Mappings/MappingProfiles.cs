using System;
using Application.DTO.CartDTO;
using Application.DTO.RoleDTO;
using Application.DTO.UserDTO;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            UserMappingRules();
            RoleMappingRules();
            AccountMappingRules();
            CartMappingRules();

        }

        private void AccountMappingRules()
        {
            CreateMap<UserRegisterDTO, User>();

        }

        private void RoleMappingRules()
        {
            CreateMap<RoleCreateDTO, Role>()
                .ForMember(dest => dest.Permissions,
                opt => opt.MapFrom(src => src.PermissionsId
                            .Select(x => new Permission() { PermissionId = x })));

            CreateMap<Role, RoleGetDTO>()
                .ForMember(dest => dest.PermissionsId,
                opt => opt.MapFrom(src => src.Permissions.Select(x => x.PermissionId)));

            CreateMap<RoleUpdateDTO, Role>()
                .ForMember(dest => dest.Permissions,
                opt => opt.MapFrom(src => src.PermissionsId
                            .Select(x => new Permission() { PermissionId = x })));
        }

        private void UserMappingRules()
        {
            CreateMap<UserCreateDTO, User>()
                .ForMember(dest => dest.Roles,
                           opt => opt.MapFrom(src => src.RoleId
                              .Select(x => new Role() { RoleId = x })));

            CreateMap<UserNewCreateDTO, User>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<User, UserGetDTO>()
                .ForMember(dest => dest.RolesId,
                           opt => opt.MapFrom(src => src.Roles.Select(x => x.RoleId)));

            CreateMap<UserUpdateDTO, User>()
               .ForMember(dest => dest.Roles,
                          opt => opt.MapFrom(src => src.RoleId
                             .Select(x => new Role() { RoleId = x })));
        }

        private void CartMappingRules()
        {
            CreateMap<Cart, CartDetailsDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));

            CreateMap<CartItem, CartItemDTO>();

            CreateMap<CartCreateDTO, Cart>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.IsOrdered, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore());

            CreateMap<CartItemAddDTO, CartItem>();
        }
    }

}

