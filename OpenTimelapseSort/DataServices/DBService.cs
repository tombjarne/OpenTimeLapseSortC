using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DbService
    {
        /// <summary>
        ///     GetImportAsync()
        ///     returns the current import from the database
        /// </summary>
        /// <returns></returns>
        public async Task<SImport> GetImportAsync()
        {
            await using var context = new ImportContext();
            var import = await context.Imports
                .SingleAsync(i => i.Timestamp == DateTime.Today);

            return import;
        }


        /// <summary>
        ///     ImportExistsAsync()
        ///     delegates request to see if a current import exists
        ///     calls <see cref="GetImportAsync" /> and returns true on success, otherwise false is returned
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     SaveImportAsync()
        ///     saves a provided <see cref="import" /> to the database
        ///     if saving fails it ensures the database is created or resets the database
        /// </summary>
        /// <param name="import"></param>
        /// <returns></returns>
        public async Task SaveImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                await database.AddAsync(import);
                await database.SaveChangesAsync();
            }
            catch
            {
                if (await database.Database.EnsureCreatedAsync())
                    await SaveImportAsync(import);
                else
                    CreateAndMigrate();
            }
        }

        /// <summary>
        ///     CreateAndMigrate()
        ///     runs the corresponding migrations to reset the database
        /// </summary>
        private static async void CreateAndMigrate()
        {
            await using var database = new ImportContext();
            await database.Database.MigrateAsync();
        }

        /// <summary>
        ///     UpdateCurrentImportAsync()
        ///     updates the values for <see cref="import" /> in the database
        /// </summary>
        /// <param name="import"></param>
        /// <returns></returns>
        public async Task UpdateCurrentImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                var entity = database.Imports
                    .Single(i => i.Timestamp == DateTime.Today);

                entity.Directories = import.Directories;

                await database.SaveChangesAsync();
            }
            catch
            {
                await SaveImportAsync(import);
            }
        }

        /// <summary>
        ///     UpdateImportAfterRemovalAsync()
        ///     handles changes to the current import after a directory has been deleted
        /// </summary>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        public async Task UpdateImportAfterRemovalAsync(string directoryId)
        {
            await using var context = new ImportContext();

            var directory = await context.ImageDirectories
                .SingleAsync(d => d.Id == directoryId);

            var import = await context.Imports
                .SingleAsync(i => i.Id == directory.ImportId);

            var images = await context.Images
                .Where(i => i.ParentDirectory.Id == directory.Id)
                .ToListAsync();

            foreach (var image in images)
            {
                directory.ImageList.Remove(image);
                context.Images.Remove(image);
            }

            import.Directories.Remove(directory);
            context.ImageDirectories.Remove(directory);
            await context.SaveChangesAsync();

            if (ImportIsEmpty(import.Id)) context.Imports.Remove(import);

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     ImportIsEmpty()
        ///     checks if the current import is empty or not and returns a corresponding result
        /// </summary>
        /// <param name="importId"></param>
        /// <returns></returns>
        private static bool ImportIsEmpty(string importId)
        {
            using var database = new ImportContext();
            var directories = database.ImageDirectories
                .Where(d => d.ImportId == importId)
                .ToListAsync();

            return directories.Result.Count == 0;
        }

        /// <summary>
        ///     UpdateDirectoryAsync()
        ///     updates a provided <see cref="directory" /> and saves it to the database
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public async Task UpdateDirectoryAsync(SDirectory directory)
        {
            await using var database = new ImportContext();
            var entity = await database.ImageDirectories
                .SingleAsync(d => d.Id == directory.Id);

            var images = await database.Images
                .Where(i => i.ParentDirectory.Id == entity.Id).ToListAsync();

            entity.ImageList = images;

            if (entity.ImageList.Count != directory.ImageList.Count) entity.ImageList = directory.ImageList;

            entity.Name = directory.Name;
            entity.Target = directory.Target;

            await database.SaveChangesAsync();
        }

        /// <summary>
        ///     GetDirectoriesAsync()
        ///     returns all existing directories from the database as a list
        /// </summary>
        /// <returns></returns>
        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            try
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
            catch (Exception)
            {
                CreateAndMigrate();
                return new List<SDirectory>();
            }
        }
    }
}