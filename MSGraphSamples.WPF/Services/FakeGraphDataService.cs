﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace MsGraph_Samples.Services
{
    public class FakeGraphDataService : IGraphDataService
    {
        public string? LastUrl => "https://graph.microsoft.com/beta/users?$count=true";

        public Task<IEnumerable<DirectoryObject>?> GetApplicationsAsync(string filter, string search, string select, string orderBy)
        {
            return Task.FromResult<IEnumerable<DirectoryObject>?>(null);
        }

        public Task<IEnumerable<DirectoryObject>?> GetDevicesAsync(string filter, string search, string select, string orderBy)
        {
            return Task.FromResult<IEnumerable<DirectoryObject>?>(null);
        }

        public Task<IEnumerable<DirectoryObject>?> GetGroupsAsync(string filter, string search, string select, string orderBy)
        {
            return Task.FromResult<IEnumerable<DirectoryObject>?>(null);
        }

        public Task<IEnumerable<DirectoryObject>?> GetAppOwnersAsUsersAsync(string id)
        {
            return Task.FromResult<IEnumerable<DirectoryObject>?>(null);
        }

        public Task<IEnumerable<DirectoryObject>?> GetTransitiveMemberOfAsGroupsAsync(string id)
        {
            return Task.FromResult<IEnumerable<DirectoryObject>?>(null);
        }

        public Task<IEnumerable<DirectoryObject>?> GetTransitiveMembersAsUsersAsync(string id)
        {
            return Task.FromResult<IEnumerable<DirectoryObject>?>(null);
        }

        public Task<IEnumerable<DirectoryObject>?> GetUsersAsync(string filter, string search, string select, string orderBy)
        {
            var directoryObjects = new[]
            {
                new User { Id = "1", DisplayName = "Luca Spolidoro", Mail = "a@b.c" },
                new User { Id = "2", DisplayName = "Pino Quercia", Mail = "pino@quercia.com" },
                new User { Id = "3", DisplayName = "Test Test", Mail = "test@test.com" }
            };

            return Task.FromResult<IEnumerable<DirectoryObject>?>(directoryObjects);
        }

        public Task<long> GetUsersRawCountAsync(string filter, string search)
        {
            return Task.FromResult(0L);
        }
    }
}