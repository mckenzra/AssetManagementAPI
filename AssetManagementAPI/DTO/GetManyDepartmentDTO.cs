﻿using System.Text.Json.Serialization;

namespace AssetManagementAPI.DTO
{
    public class GetManyDepartmentDTO
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public int ItemCount { get; }
        [JsonPropertyName("data")]
        public IEnumerable<GetDepartmentDTO> Departments { get; }

        public GetManyDepartmentDTO(int pageNumber, int pageSize, int itemCount, IEnumerable<GetDepartmentDTO> departments)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.ItemCount = itemCount;
            this.Departments = departments;
        }
    }
}
