using HarmonyHome.Wpf.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarmonyHome.Wpf.Services.Interfaces
{
    public interface IUbicacionService
    {


        Task<List<UbicacionDTO>> GetUbicacionesAsync();
    }
}