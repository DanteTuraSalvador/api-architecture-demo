using System.Net.Http.Json;
using DemoApi.BlazorClient.Models;

namespace DemoApi.BlazorClient.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Vehicle>>("/api/vehicles") ?? new List<Vehicle>();
    }

    public async Task<Vehicle?> GetVehicleAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<Vehicle>($"/api/vehicles/{id}");
    }

    public async Task<List<Vehicle>> GetVehiclesByStatusAsync(string status)
    {
        return await _httpClient.GetFromJsonAsync<List<Vehicle>>($"/api/vehicles/status/{status}") ?? new List<Vehicle>();
    }

    public async Task<Vehicle?> CreateVehicleAsync(Vehicle vehicle)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/vehicles", vehicle);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Vehicle>();
    }

    public async Task<Vehicle?> UpdateVehicleAsync(Guid id, Vehicle vehicle)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/vehicles/{id}", vehicle);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Vehicle>();
    }

    public async Task DeleteVehicleAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/api/vehicles/{id}");
        response.EnsureSuccessStatusCode();
    }
}
