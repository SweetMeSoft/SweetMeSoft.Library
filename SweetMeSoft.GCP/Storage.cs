using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

using SweetMeSoft.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SweetMeSoft.GCP
{
    internal class Storage
    {

        private static Storage instance;

        private readonly StorageClient storage;

        public static Storage Instance => instance ??= new Storage();

        public Storage()
        {
            var gc = GoogleCredential.FromFile("intl-rosa-sandbox-7936597cb05b.json");
            storage = StorageClient.Create(gc);
        }

        public async Task<StreamFile> DownloadFile(string bucket, string path, string filename)
        {
            try
            {
                path = path.EndsWith("/") ? path : path + "/";
                var stream = new MemoryStream();
                var file = await storage.DownloadObjectAsync(bucket, path + filename, stream);
                var extension = filename.ToLower().Substring(filename.LastIndexOf('.') + 1);
                stream.Position = 0;
                return new StreamFile(filename, stream, Constants.ContentTypesDict.FirstOrDefault(model => model.Key.ToString().ToLower() == extension).Key);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("File " + path + filename + " not found.");
                return null;
            }
        }

        public async Task<List<StreamFile>> DownloadFiles(string bucket, string path, bool recurring = false)
        {
            path = path.EndsWith("/") ? path : path + "/";
            var totalFiles = 0;
            var stream = new MemoryStream();
            var files = new List<StreamFile>();
            var filenames = await storage.ListObjectsAsync(bucket, path).ToListAsync();
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            };
            Parallel.ForEach(filenames, parallelOptions, async filename =>
            {
                try
                {
                    var name = filename.Name.Replace(path, "");
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (recurring)
                        {
                            files.Add(DownloadFile(bucket, path, name).Result);
                            totalFiles++;
                        }
                        else
                        {
                            if (!name.Contains("/"))
                            {
                                files.Add(DownloadFile(bucket, path, name).Result);
                                totalFiles++;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message + ". File: " + filename);
                }
            });

            Console.WriteLine("Downloaded " + totalFiles + " files in " + path + ".");
            return files;
        }

        public async Task<bool> UploadFile(string bucket, string path, StreamFile file)
        {
            try
            {
                path = path.EndsWith("/") ? path : path + "/";
                await storage.UploadObjectAsync(bucket, path + file.FileName, Constants.ContentTypesDict[file.ContentType], file.Stream);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: " + e.Message + ". File: " + file.FileName);
                return false;
            }
        }

        public int UploadFiles(string bucket, string path, List<StreamFile> files)
        {
            var totalFiles = files.Count;
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            };
            Parallel.ForEach(files, parallelOptions, async streamFile =>
            {
                if (!UploadFile(bucket, path, streamFile).Result)
                {
                    totalFiles--;
                }
            });

            Console.WriteLine("Uploaded " + totalFiles + " of " + files.Count + " files.");
            return totalFiles;
        }

        public async Task<bool> DeleteFile(string bucket, string path, string filename)
        {
            try
            {
                path = path.EndsWith("/") ? path : path + "/";
                await storage.DeleteObjectAsync(bucket, path + filename);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return false;
            }
        }

        public int DeleteFiles(string bucket, string path, IEnumerable<string> filenames)
        {
            var totalFiles = 0;
            path = path.EndsWith("/") ? path : path + "/";
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            };
            Parallel.ForEach(filenames, parallelOptions, filename =>
            {
                try
                {
                    if (DeleteFile(bucket, path, filename).Result)
                    {
                        totalFiles++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message + ". File: " + filename);
                }
            });

            Console.WriteLine("Deleted " + totalFiles + " files.");
            return totalFiles;
        }

        public async Task<bool> MoveFile(string bucket, string path, string newBucket, string newPath, StreamFile file)
        {
            try
            {
                path = path.EndsWith("/") ? path : path + "/";
                newPath = newPath.EndsWith("/") ? newPath : newPath + "/";
                if (await UploadFile(newBucket, newPath, file))
                {
                    await DeleteFile(bucket, path, file.FileName);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error moving: " + e.Message + ". File: " + file.FileName);
                return false;
            }
        }

        public int MoveFiles(string bucket, string path, string newBucket, string newPath, List<StreamFile> files)
        {
            var totalFiles = files.Count;
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            };
            Parallel.ForEach(files, parallelOptions, async streamFile =>
            {
                if (!MoveFile(bucket, path, newBucket, newPath, streamFile).Result)
                {
                    totalFiles--;
                }
            });

            Console.WriteLine("Moved " + totalFiles + " of " + files.Count + " files.");
            return totalFiles;
        }
    }
}
