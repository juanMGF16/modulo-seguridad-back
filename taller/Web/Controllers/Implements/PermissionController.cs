﻿using Business.Interfaces.IBusinessImplements;
using Entity.Domain.Enums;
using Entity.DTOs.Default;
using Entity.DTOs.Select;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.ControllersBase.Web.Controllers.BaseController;

namespace Web.Controllers.Implements
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    public class PermissionController : BaseController<PermissionSelectDto,PermissionDto, IPermissionService>
    {
        public PermissionController(IPermissionService service, ILogger<PermissionController> logger) : base(service, logger)
        {
        }

        protected override Task AddAsync(PermissionDto dto) => _service.CreateAsync(dto);

        protected override Task<bool> DeleteAsync(int id, DeleteType deleteType) => _service.DeleteAsync(id, deleteType);

        protected override Task<IEnumerable<PermissionSelectDto>> GetAllAsync(GetAllType getAllType) => _service.GetAllAsync(getAllType);

        protected override Task<PermissionSelectDto?> GetByIdAsync(int id) => _service.GetByIdAsync(id);

        protected override Task<bool> RestaureAsync(int id) => _service.RestoreLogical(id);

        protected override Task<bool> UpdateAsync(int id, PermissionDto dto) => _service.UpdateAsync(dto);
    }
}
