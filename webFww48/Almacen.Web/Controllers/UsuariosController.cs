using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Almacen.Web.Models;

using Almacen.Web.Tools;
using RedWillow.MvcToastrFlash;
using Almacen.Web.Aplicacion;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador)]
    public class UsuariosController : Controller
    {
        
        public UsuariosController() { }

        public UsuariosController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Users
        [HttpGet]
        public async Task<ActionResult> Index()
        {

            List<UsuarioViewModel> lista = await UserManager.Users.Select(s=> 
            new UsuarioViewModel {
                Id=s.Id,
                UserName=s.UserName,
                NombreCompleto=s.NombreCompleto,
                Email =s.Email
                }
            ).ToListAsync();

            lista.ForEach(f => f.RolesList = UserManager.GetRoles(f.Id).Select(r => new SelectListItem { Text = r }).ToList());

            return View(lista);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string textobuscar)
        {

            List<UsuarioViewModel> lista = await UserManager.Users.Where(w => 
                w.NombreCompleto.Contains(textobuscar) || 
                w.UserName.Contains(textobuscar) ||
                w.Email.Contains(textobuscar)
            ).Select(s =>
            new UsuarioViewModel
            {
                Id = s.Id,
                UserName = s.UserName,
                NombreCompleto = s.NombreCompleto,
                Email = s.Email
            }
            ).ToListAsync();

            lista.ForEach(f => f.RolesList = UserManager.GetRoles(f.Id).Select(r => new SelectListItem { Text = r }).ToList());

            ViewBag.textobuscar = textobuscar;

            return View(lista);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            ApplicationUser aspNetUsers = await UserManager.FindByIdAsync(id);
            if (aspNetUsers == null)
            {
                return RedirectToAction("NotFound","Error",null);
            }

            UsuarioViewModel dtg = new UsuarioViewModel
            {
                Id = aspNetUsers.Id,
                UserName = aspNetUsers.UserName,
                Email = aspNetUsers.Email,
                NombreCompleto = aspNetUsers.NombreCompleto

            };
            return View(dtg);
        }

        // GET: Users/Create
        public async Task<ActionResult> Create()
        {

            SelectList listaRoles = await CustomExtension.convertSelectListAsyc(await RoleManager.Roles.ToListAsync(), "Name", "Name", false);

            UsuarioViewModel model = new UsuarioViewModel();
            model.RolesList = listaRoles;
            ViewBag.userRoles = new List<string>();

            return View(model);
        }

        // POST: Users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UsuarioViewModel model, string[] selectedRoles)
        {
            bool resultError = false;

            SelectList listaRoles = await CustomExtension.convertSelectListAsyc(await RoleManager.Roles.ToListAsync(), "Name", "Name", false);

            model.RolesList = listaRoles;

            if (selectedRoles==null)
            {
                ModelState.AddModelError("Rol", "Seleccione un Rol");
            }

            if (ModelState.IsValid)
            {
                //Validar si existe usuario
                ApplicationUser existuser = UserManager.FindByName(model.UserName);
                if(existuser!=null)
                {
                    ModelState.AddModelError("","El usuario ya existe");
                    resultError = true;
                }

                if (resultError == false)
                {

                    //Agregar usuario
                    ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = false, NombreCompleto=model.NombreCompleto };

                    var adminresult = await UserManager.CreateAsync(user, model.Password);

                    if (!adminresult.Succeeded)
                    {
                        ModelState.AddModelError("", adminresult.Errors.First());
                        resultError = true;
                        
                    }

                    if (resultError == false)
                    {
                        if (selectedRoles != null)
                        {
                            //Agregar Roles
                            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles); //multiples roles
                            //var result = await UserManager.AddToRoleAsync(user.Id, model.Rol); //solo un rol
                            if (!result.Succeeded)
                            {
                                ModelState.AddModelError("", result.Errors.First());
                                resultError = true;

                            }
                        }
                    }

                }

                if (resultError == false)
                {
                    string mensaje = "El Registro se guardo correctamente!";
                    this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
                    return RedirectToAction("Index");
                }
            }

            ViewBag.userRoles = selectedRoles.ToList();

            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }

            ApplicationUser aspNetUsers = await UserManager.FindByIdAsync(id);
            if (aspNetUsers == null)
            {
                return RedirectToAction("NotFound","Error",null);
            }

            var userRoles = await UserManager.GetRolesAsync(aspNetUsers.Id);


            UsuarioViewModel model = new UsuarioViewModel
            {
                Id = aspNetUsers.Id,
                UserName = aspNetUsers.UserName,
                Email = aspNetUsers.Email,
                NombreCompleto = aspNetUsers.NombreCompleto,
            };

            //model.Rol = userRoles.DefaultIfEmpty().FirstOrDefault();
            

            var Roles = await RoleManager.Roles.ToListAsync();
            SelectList listaRoles = await CustomExtension.convertSelectListAsyc(Roles, "Name", "Name", false);

            model.RolesList = listaRoles;
            ViewBag.userRoles = userRoles;


            return View(model);
        }

        // POST: Users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UsuarioViewModel model, bool cambiarPassword, string[] selectedRoles)
        {

            bool resultError = false;

            if (!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Email))
            {
                if (cambiarPassword==false)
                {
                    ModelState["Password"].Errors.Clear();
                }
            }

            if (selectedRoles == null)
            {
                ModelState.AddModelError("Rol", "Seleccione un Rol");
            }

            IList<string> userRoles = await UserManager.GetRolesAsync(model.Id);


            SelectList listaRoles = await CustomExtension.convertSelectListAsyc(await RoleManager.Roles.ToListAsync(), "Name", "Name", false);

            model.RolesList = listaRoles;
            ViewBag.userRoles = userRoles;

            if (ModelState.IsValid)
            {

                ApplicationUser user = await UserManager.FindByIdAsync(model.Id);

                if (user.UserName != model.UserName || user.Email != model.Email || user.NombreCompleto != model.NombreCompleto)//en caso de cambiar nombre de usuario o correo
                {
                    //Validar si existe usuario
                    ApplicationUser existuser = UserManager.FindByName(model.UserName);
                    if (existuser != null)
                    {
                        if (existuser.Id!=model.Id)
                        {
                            ModelState.AddModelError("", "El usuario ya existe");
                            resultError = true;
                        }
                    }
                    

                    if(resultError == false)
                    {

                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        user.NombreCompleto = model.NombreCompleto;

                        var result = await UserManager.UpdateAsync(user);

                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            resultError = true;
                        }
                    }

                }

                if (cambiarPassword) //en caso de cambiar el Password
                {
                    var resultremovepassw = UserManager.RemovePassword(user.Id);

                    if (!resultremovepassw.Succeeded)
                    {
                        ModelState.AddModelError("", resultremovepassw.Errors.First());
                        resultError = true;
                    }

                    if (resultError == false)
                    {
                        var resultpassw = await UserManager.AddPasswordAsync(user.Id, model.Password);

                        if (!resultpassw.Succeeded)
                        {
                            ModelState.AddModelError("", resultpassw.Errors.First());
                            resultError = true;
                        }
                    }
                }
                

                
                //IList<string> RolSeleccionado = new List<string>();

                if (resultError == false)
                {

                    //Busca Roles asignados

                    //para varios roles selectedRoles = selectedRole ?? new string[] { };
                    var addRoles = selectedRoles.Except(userRoles).ToArray<string>();

                    // para un rol
                    //RolSeleccionado.Add(model.Rol); ;
                    //var addRoles = RolSeleccionado.Except(userRoles).ToArray<string>();

                    //roles a agregar
                    if (addRoles.Length > 0)
                    {
                        var resultAdd = await UserManager.AddToRolesAsync(user.Id, addRoles);
                        //var resultAdd = await UserManager.AddToRoleAsync(user.Id, model.Rol);

                        if (!resultAdd.Succeeded)
                        {
                            ModelState.AddModelError("", resultAdd.Errors.First());
                            resultError = true;
                        }
                    }
                }

                if (resultError == false)
                {
                    //roles a quitar
                    var removeRoles = userRoles.Except(selectedRoles).ToArray<string>();
                    if (removeRoles.Length > 0)
                    {
                        var resultRemove = await UserManager.RemoveFromRolesAsync(user.Id, removeRoles);

                        if (!resultRemove.Succeeded)
                        {
                            ModelState.AddModelError("", resultRemove.Errors.First());
                            resultError = true;
                        }
                    }
                }

                if (resultError == false)
                {
                    string mensaje = "El Registro se guardo correctamente!";
                    this.Flash(Toastr.SUCCESS, "Mensaje", mensaje);
                    return RedirectToAction("Index");
                }
            }

            

            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            ApplicationUser aspNetUsers = await UserManager.FindByIdAsync(id);
            if (aspNetUsers == null)
            {
                return RedirectToAction("NotFound","Error",null);
            }


            UsuarioViewModel dtg = new UsuarioViewModel
            {
                Id = aspNetUsers.Id,
                UserName = aspNetUsers.UserName,
                Email = aspNetUsers.Email,
                NombreCompleto = aspNetUsers.NombreCompleto
            };

            return View(dtg);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {

            ApplicationUser aspNetUsers = await UserManager.FindByIdAsync(id);
            
            await UserManager.DeleteAsync(aspNetUsers);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
