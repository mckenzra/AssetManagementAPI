﻿using AssetManagementAPI.DTO;
using AssetManagementAPI.Interfaces;
using AssetManagementAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/departments")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IValidator<CreateDepartmentDTO> _createDepartmentValidator;
        private readonly IValidator<UpdateDepartmentDTO> _updateDepartmentValidator;
        private readonly IValidator<QueryObject> _queryObjectValidator;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentRepository departmentRepository,
            IValidator<CreateDepartmentDTO> createDepartmentValidator,
            IValidator<UpdateDepartmentDTO> updateDepartmentValidator,
            IValidator<QueryObject> queryObjectValidator,
            ILogger<DepartmentController> logger)
        {
            this._departmentRepository = departmentRepository;
            this._createDepartmentValidator = createDepartmentValidator;
            this._updateDepartmentValidator = updateDepartmentValidator;
            this._queryObjectValidator = queryObjectValidator;
            this._logger = logger;
        }

        [HttpGet(Name = "IndexDepartments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetManyDepartmentsDTO>> IndexAsync([FromQuery] QueryObject? queryObject)
        {
            if (queryObject != null)
            {
                ValidationResult validationResult = await _queryObjectValidator.ValidateAsync(queryObject);

                foreach (var error in validationResult.Errors)
                {
                    if (error.ErrorCode == "QERR0001")
                    {
                        queryObject.PageNumber = null;
                    }

                    if (error.ErrorCode == "QERR0002")
                    {
                        queryObject.PageSize = null;
                    }
                }
            }

            var departments = await _departmentRepository.GetAllAsync(queryObject);

            /*return Ok(departments.Select(d => d.ToDto()));*/

            return Ok(new GetManyDepartmentsDTO(
                pageNumber: departments.PageNumber,
                pageSize: departments.PageSize,
                itemCount: departments.ItemCount,
                departments: departments.Data.Select(d => d.ToDto())
            ));
        }

        [HttpPost(Name = "CreateDepartment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetDepartmentDTO>> CreateAsync([FromBody] CreateDepartmentDTO department)
        {
            ValidationResult validationResult = await _createDepartmentValidator.ValidateAsync(department);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            department.Name = department.Name?.Trim();

            Department? response = await _departmentRepository.CreateAsync(department);
            return response == null ? BadRequest(ModelState) : CreatedAtAction(nameof(ShowAsync), new { id = response.Id }, response.ToDto());
        }

        [HttpGet("{id}", Name = "ShowDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetDepartmentDTO>> ShowAsync([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Null or invalid id");
            }

            Department? department = await _departmentRepository.GetByIdAsync(id);

            return department == null ? NotFound() : Ok(department.ToDto());
        }

        [HttpPut("{id}", Name = "UpdateDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetDepartmentDTO>> UpdateAsync([FromRoute] string id, [FromBody] UpdateDepartmentDTO department)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Null or invalid id");
            }

            ValidationResult validationResult = await _updateDepartmentValidator.ValidateAsync(department);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            department.Name = department.Name?.Trim();

            Department? response = await _departmentRepository.UpdateAsync(id, department);
            return response == null ? NotFound() : Ok(response.ToDto());
        } 

        [HttpDelete("{id}", Name = "DestroyDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetDepartmentDTO>> DestroyAsync([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Null or invalid id");
            }

            Department? response = await _departmentRepository.DeleteAsync(id);
            return response == null ? NotFound() : Ok(response.ToDto());
        }
    }
}
