using ConfiscationService.Models;

namespace ConfiscationService.DTOs;

public record ConfiscationOrderDto(LicensePlate LicensePlate, ConfiscationReason ConfiscationReason, string Comment, Address ImpoundYardAddress);