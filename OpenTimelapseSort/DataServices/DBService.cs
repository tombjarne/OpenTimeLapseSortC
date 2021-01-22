﻿using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DbService
    {

        /**
         * ReturnCurrentImport
         * 
         * Returns the latest Import instance including its directories
         * 
         */

        public async Task<SImport> GetImportAsync()
        {
            await using var context = new ImportContext();

            var import = await context.Imports
                .SingleAsync(i => i.Timestamp == DateTime.Today);

            return import;
        }

        public async Task<bool> ImportExistsAsync()
        {
            try
            {
                await GetImportAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SaveImportAsync(SImport import)
        {
            await using var database = new ImportContext();

            try
            {
                await database.AddAsync(import);
                await database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.StackTrace);
            }
        }

        public async Task UpdateImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                var entity = database.Imports
                    .Single(i => i.Timestamp == DateTime.Today);

                entity.Directories = import.Directories;
                entity.Length = import.Length;

                await database.SaveChangesAsync();
            }
            catch (Exception)
            {
                // handle exception
            }
        }

        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {

            await using var context = new ImportContext();

            var directories = await context.ImageDirectories
                .ToListAsync();

            foreach (var directory in directories)
            {
                directory.ImageList = await context.Images
                    .Where(i => i.DirectoryId == directory.Id)
                    .ToListAsync();

                directory.ParentImport = await context.Imports
                    .SingleAsync(i => i.Id == directory.ImportId);
            }

            return directories;
        }
    }
}
