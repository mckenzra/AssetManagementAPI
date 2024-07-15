﻿using AssetManagementAPI.DTO;
using FluentValidation;

namespace AssetManagementAPI.Services.Validation
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDTO>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .OverridePropertyName("name")
                .WithMessage("Name is required.");
        }
    }
}
