﻿using AssetManagementAPI.DTO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AssetManagementAPI.Models
{
    public class Asset : IDisposable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public required string Id { get; set; }
        public string? Type { get; set; }
        public required string Name { get; set; }
        public JsonDocument? Info { get; set; }
        public Department? Proprietor { get; set; }
        public Employee? Custodian { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }

        public Asset()
        {
            Transactions = [];
            MaintenanceRecords = [];
        }

        public GetAssetDTO? ToDto()
        {
            return new GetAssetDTO(
                id: this.Id,
                type: this.Type,
                name: this.Name,
                info: this.Info,
                proprietorId: this.Proprietor?.Id,
                custodianId: this.Custodian?.Id,
                isActive: this.IsActive
            );
        }

        public void Dispose()
        {
            Info?.Dispose();
        }
    }
}
