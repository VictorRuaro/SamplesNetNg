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
using Almacen.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using Almacen.Web.Aplicacion;

namespace Almacen.Web.Controllers
{
    [Authorize(Roles = EnumRol.Administrador)]
    public class RolesController : Controller
    {
        public RolesController() { }
        public RolesController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
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
        // GET: Roles
        public async Task<ActionResult> Index()
        {
            List<RolViewModel> lista = await RoleManager.Roles.Select(
                s=> new RolViewModel
                {
                     Id=s.Id,
                     Rol=s.Name
                }
                ).ToListAsync() ;
            return View(lista);
        }

        // GET: Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            IdentityRole rol = await RoleManager.FindByIdAsync(id);
            if (rol == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            RolViewModel model = new RolViewModel();
            model.Id = rol.Id;
            model.Rol = rol.Name;

            return View(model);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RolViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IdentityRole rol = new IdentityRole();
                    
                    rol.Name = model.Rol;

                    var result = await RoleManager.CreateAsync(rol);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        

                    }
                }
                catch (DbEntityValidationException entex)
                {
                    string error = entex.Message;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Roles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            IdentityRole rol = await RoleManager.FindByIdAsync(id);
            if (rol == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            RolViewModel model = new RolViewModel();
            model.Id = rol.Id;
            model.Rol = rol.Name;

            return View(model);

        }

        // POST: Roles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RolViewModel model)
        {
            if (ModelState.IsValid)
            {

                var rol = RoleManager.FindById(model.Id);
                rol.Name = model.Rol;

                var result = await RoleManager.UpdateAsync(rol);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error", null);
            }
            IdentityRole rol = await RoleManager.FindByIdAsync(id);
            if (rol == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            RolViewModel model = new RolViewModel();
            model.Id = rol.Id;
            model.Rol = rol.Name;

            return View(model);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {

            var rol = RoleManager.FindById(id);

            if (rol == null)
            {
                return RedirectToAction("NotFound", "Error", null);
            }

            var result = await RoleManager.DeleteAsync(rol);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
            base.Dispose(disposing);
        }
    }
}
