using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TelNet.Repositories;

namespace TelNet.Controllers
{
    public class DocumentManagementController : Controller
    {
        private readonly BaseRepository<Folders> _foldersRepo;
        private readonly BaseRepository<Files> _filesRepo;

        public DocumentManagementController()
        {
            _foldersRepo = new BaseRepository<Folders>();
            _filesRepo = new BaseRepository<Files>();
        }

        public ActionResult Index(int? folderId)
        {
            var model = new FolderViewModel();

            if (folderId.HasValue)
            {
                model.CurrentFolder = _foldersRepo.Get(folderId.Value);
                model.SubFolders = _foldersRepo.GetAll().Where(f => f.ParentFolderId == folderId).ToList();
                model.Files = _filesRepo.GetAll().Where(f => f.FolderId == folderId).ToList();
            }
            else
            {
                model.SubFolders = _foldersRepo.GetAll().Where(f => f.ParentFolderId == null).ToList();
            }

            // Initialize Files list if null
            if (model.Files == null)
            {
                model.Files = new List<Files>();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteFolder(int folderId)
        {
            try
            {
                var folderToDelete = _foldersRepo.Get(folderId);
                if (folderToDelete != null)
                {
                    // Delete associated files
                    var filesToDelete = _filesRepo.Table.Where(f => f.FolderId == folderId);
                    foreach (var file in filesToDelete)
                    {
                        _filesRepo.Delete(file.Id);
                    }

                    // Delete subfolders recursively
                    DeleteSubfolders(folderId);

                    // Delete the folder itself
                    _foldersRepo.Delete(folderId);
                }

                return RedirectToAction("Index", new { folderId = folderToDelete?.ParentFolderId });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return RedirectToAction("Index", new { folderId });
            }
        }

        private void DeleteSubfolders(int folderId)
        {
            var subfoldersToDelete = _foldersRepo.Table.Where(f => f.ParentFolderId == folderId);
            foreach (var subfolder in subfoldersToDelete)
            {
                // Delete associated files
                var filesToDelete = _filesRepo.Table.Where(f => f.FolderId == subfolder.Id);
                foreach (var file in filesToDelete)
                {
                    _filesRepo.Delete(file.Id);
                }

                // Delete subfolders recursively
                DeleteSubfolders(subfolder.Id);

                // Delete the subfolder itself
                _foldersRepo.Delete(subfolder.Id);
            }
        }

        [HttpPost]
        public ActionResult RenameFolder(int folderId, string newName)
        {
            try
            {
                var folderToRename = _foldersRepo.Get(folderId);
                if (folderToRename != null)
                {
                    folderToRename.Name = newName;
                    _foldersRepo.Update(folderToRename.Id, folderToRename);
                }

                return RedirectToAction("Index", new { folderId });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return RedirectToAction("Index", new { folderId });
            }
        }

        [HttpPost]
        public ActionResult CreateFile(int folderId, string fileName)
        {
            try
            {
                var newFile = new Files
                {
                    Name = fileName,
                    FolderId = folderId
                };

                _filesRepo.Create(newFile);

                return RedirectToAction("Index", new { folderId });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return RedirectToAction("Index", new { folderId });
            }
        }

        [HttpPost]
        public ActionResult CreateFolder(int? parentFolderId, string folderName)
        {
            try
            {
                if (!parentFolderId.HasValue)
                {
                    // Handle the case where the parent folder ID is not provided
                    // For example, create the folder in the root folder

                    var rootFolder = _foldersRepo.GetAll().FirstOrDefault(f => f.ParentFolderId == null);
                    if (rootFolder == null)
                    {
                        // If root folder doesn't exist, create it
                        rootFolder = new Folders { Name = "Root" };
                        _foldersRepo.Create(rootFolder);
                    }

                    var newFolders = new Folders
                    {
                        Name = folderName,
                        ParentFolderId = rootFolder.Id
                    };

                    _foldersRepo.Create(newFolders);

                    return RedirectToAction("Index", new { folderId = rootFolder.Id });
                }

                var parentFolder = _foldersRepo.Get(parentFolderId.Value);
                if (parentFolder == null)
                {
                    // Handle the case where the specified parent folder doesn't exist
                    // For example, redirect to the root folder
                    return Json(new { success = false, message = "Parent folder not found" });
                }

                var newFolder = new Folders
                {
                    Name = folderName,
                    ParentFolderId = parentFolderId.Value
                };

                _foldersRepo.Create(newFolder);

                return RedirectToAction("Index", new { folderId = parentFolderId });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteFile(int fileId)
        {
            try
            {
                var fileToDelete = _filesRepo.Get(fileId);
                if (fileToDelete != null)
                {
                    fileToDelete.IsDeleted = true;
                    _filesRepo.Update(fileId, fileToDelete);
                    return RedirectToAction("Index", new { folderId = fileToDelete.FolderId });
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult RenameFile(int fileId, string newName)
        {
            try
            {
                var fileToRename = _filesRepo.Get(fileId);
                if (fileToRename != null)
                {
                    fileToRename.Name = newName;
                    _filesRepo.Update(fileToRename.Id, fileToRename);
                }

                return RedirectToAction("Index", new { folderId = fileToRename?.FolderId });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult RestoreFile(int fileId)
        {
            try
            {
                var fileToRestore = _filesRepo.Get(fileId);
                if (fileToRestore != null && fileToRestore.IsDeleted)
                {
                    fileToRestore.IsDeleted = false;
                    _filesRepo.Update(fileId, fileToRestore);
                    return RedirectToAction("Index", new { folderId = fileToRestore.FolderId });
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return RedirectToAction("Index");
            }
        }


        public class FolderViewModel
        {
            public Folders CurrentFolder { get; set; }
            public List<Folders> SubFolders { get; set; }
            public List<Files> Files { get; set; }
        }

        public ActionResult Trash()
        {
            var deletedFiles = _filesRepo.GetAll().Where(f => f.IsDeleted).ToList();
            return View(deletedFiles);
        }

        [HttpPost]
        public ActionResult PermanentlyDeleteFile(int fileId)
        {
            try
            {
                var fileToDelete = _filesRepo.Get(fileId);
                if (fileToDelete != null)
                {
                    _filesRepo.Delete(fileId);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "File not found" });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}

