﻿using GestionVoluntariadoEventosGUI.Models;
using GestionVoluntariadoEventosGUI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestionVoluntariadoEventosGUI.Servicios
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public ApiService(string baseApiUrl) // https://localhost:7121/
        {
            _httpClient = new HttpClient();
            _baseApiUrl = baseApiUrl;
            _httpClient.BaseAddress = new System.Uri(_baseApiUrl);
        }

        //Método para el Login
        public async Task<(User? User, string? ErrorMessage)> LoginAsync (string username, string password)
        {
            var loginRequest = new LoginRequest { UserName = username, Password = password };
            var jsonContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Users/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (user, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // Aquí podrías parsear el error si tu API devuelve un formato de error específico
                    return (null, $"Error al iniciar sesión: {response.ReasonPhrase} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Error de conexión: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return (null, $"Error de formato de datos: {ex.Message}");
            }
        }

        public async Task<string?> RegisterUserAsync(User user)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync("api/Users", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return null; // Éxito, no hay mensaje de error
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error al registrar usuario: {response.ReasonPhrase} - {errorContent}";
                }
            }
            catch (HttpRequestException ex)
            {

                return $"Error de conexión: {ex.Message}";
            }
            catch (JsonException ex)
            {
                return $"Error de formato de datos: {ex.Message}";
            }
        }

        // Método para crear un nuevo evento (similar a RegisterUserAsync)
        public async Task<string?> CreateEventAsync(Event @event)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(@event), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Events", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return null; // Éxito
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error al crear evento: {response.ReasonPhrase} - {errorContent}";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error de conexión: {ex.Message}";
            }
            catch (JsonException ex)
            {
                return $"Error de formato de datos: {ex.Message}";
            }
        }

        public async Task<string> RegisterVolunteerAsync(VolunteerDto volunteerDto)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(volunteerDto), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Volunteers", jsonContent); // Endpoint de VolunteersController

                if (response.IsSuccessStatusCode)
                {
                    return null; // Éxito
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error al registrar voluntario: {response.ReasonPhrase} - {errorContent}";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error de conexión: {ex.Message}";
            }
            catch (JsonException ex)
            {
                return $"Error de formato de datos: {ex.Message}";
            }
        }

        // Nuevo método para obtener todos los voluntarios (necesario para la asignación)
        public async Task<(List<Volunteer>? Volunteers, string? ErrorMessage)> GetVolunteersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Volunteers"); // Endpoint de VolunteersController

                if (response.IsSuccessStatusCode)
                {
                    var volunteersJson = await response.Content.ReadAsStringAsync();
                    var volunteers = JsonSerializer.Deserialize<List<Volunteer>>(volunteersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (volunteers, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (null, $"Error al obtener voluntarios: {response.ReasonPhrase} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Error de conexión: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return (null, $"Error de formato de datos: {ex.Message}");
            }
        }

        // Nuevo método para obtener eventos (necesario para la asignación)
        public async Task<(List<Event>? Events, string? ErrorMessage)> GetEventsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Events"); // Endpoint de EventsController

                if (response.IsSuccessStatusCode)
                {
                    var eventsJson = await response.Content.ReadAsStringAsync();
                    var events = JsonSerializer.Deserialize<List<Event>>(eventsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (events, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (null, $"Error al obtener eventos: {response.ReasonPhrase} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Error de conexión: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return (null, $"Error de formato de datos: {ex.Message}");
            }
        }

        // Nuevo método para asignar voluntario a evento
        public async Task<string?> AssignVolunteerToEventAsync(int eventId, int volunteerId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Events/{eventId}/assign-volunteer/{volunteerId}", null); // 

                if (response.IsSuccessStatusCode)
                {
                    return null; // Éxito
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error al asignar voluntario: {response.ReasonPhrase} - {errorContent}";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error de conexión: {ex.Message}";
            }
            catch (JsonException ex)
            {
                return $"Error de formato de datos: {ex.Message}";
            }
        }


        // Puedes agregar más métodos para GET, PUT, DELETE si los necesitas en el frontend

    }
}
