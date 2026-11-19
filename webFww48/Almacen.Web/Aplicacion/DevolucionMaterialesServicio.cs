using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Almacen.Infraestructura;
using AutoMapper;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Web;
using Almacen.Web.ViewModels;
using Almacen.Web.ViewModels.Catalogos;
using Almacen.Data;
using Almacen.Web.Aplicacion;

namespace Almacen.Web.Applicacion
{
    public class DevolucionMaterialesServicio : IDisposable
    {
        private readonly IGenericUnitOfWork _UoW;

        public DevolucionMaterialesServicio(DbContext entities)
        {
            _UoW = new GenericUnitOfWork(entities);

        }
        public DevolucionMaterialesServicio(IGenericUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public DevolucionMaterialVM Agregar(DevolucionMaterialVM entity)
        {
            DevolucionMaterialVM result = null;

            AlmacenMaterialVM lote = new AlmacenMaterialVM();
            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                lote = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                    .EncontrarPor(w => w.idAlmacen == entity.idAlmacen).FirstOrDefault();
            }

            var tmp = AutoMapperGenericsHelper<AlmacenMaterialVM, DevolucionMaterialVM>.ConvertToDBEntity(lote);

            tmp.Existencia = entity.Existencia;
            tmp.IntegrarAlmacen = entity.IntegrarAlmacen;

            tmp.tSustanciaMaterial = null;
            tmp.cCondicionesAlmacenaje = null;
            tmp.cMarca = null;
            tmp.cPresentacion = null;
            tmp.cProveedor = null;
            

            entity = tmp;

            entity.FG = DateTime.Now;
            entity.UG= HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            entity.UM = null;
            entity.FM = null;
            entity.UB = null;
            entity.FB = null;


            entity.CantidadIngresa = entity.Existencia;

            tDevolucionMaterial convert = AutoMapperGenericsHelper<DevolucionMaterialVM, tDevolucionMaterial>.ConvertToDBEntity(entity);

            _UoW.Repository<tDevolucionMaterial>().Agregar(convert);

            /*Agrega un movimiento al almacen*/

            //if(entity.IntegrarAlmacen)
            //{
            //    tAlmacenMaterial almacen = AutoMapperGenericsHelper<tDevolucionMaterial, tAlmacenMaterial>.ConvertToDBEntity(convert);


            //    tAlmacenMovimiento mov = new tAlmacenMovimiento();

            //    mov.Cantidad = almacen.CantidadIngresa;
            //    mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Devolucion;

            //    mov.FG = DateTime.Now;
            //    mov.UG = HttpContext.Current.User.Identity.GetUserId();
            //    mov.ST = true;

            //    almacen.tAlmacenMovimiento.Add(mov);

            //    _UoW.Repository<tAlmacenMaterial>().Agregar(almacen);

            //}

            /* Se actualiza la existencia en el almacen del lote seleccionado*/
            if (entity.IntegrarAlmacen)
            {
                tAlmacen almacen = _UoW.Repository<tAlmacen>().ObtenerPorId(convert.idAlmacen);


                almacen.Existencia += convert.Existencia;

                tAlmacenMovimiento mov = new tAlmacenMovimiento();

                mov.Cantidad = almacen.CantidadIngresa;
                mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Devolucion;

                mov.FG = DateTime.Now;
                mov.UG = HttpContext.Current.User.Identity.GetUserId();
                mov.ST = true;

                almacen.tAlmacenMovimiento.Add(mov);

                _UoW.Repository<tAlmacen>().Editar(almacen);
            }



            result = AutoMapperGenericsHelper<tDevolucionMaterial, DevolucionMaterialVM>.ConvertToDBEntity(convert);

            return result;
        }

        public int Contar(Expression<Func<MaterialVM, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<MaterialVM, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<tDetalleMaterial, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<MaterialVM, bool>>, Expression<Func<tDetalleMaterial, bool>>>.ConvertToDBEntity(where);

            _UoW.Repository<tDetalleMaterial>().Contar(domainPredicate);

            return result;
        }

        public void Editar(DevolucionMaterialVM entity,int id)
        {

            tDevolucionMaterial tmp = null;
            using (CndAlmacenDopajeEntities bd = new CndAlmacenDopajeEntities())
            {
                tmp = new RepositoryGenerico<tDevolucionMaterial>(bd).ObtenerPorId(id);
            }

            //DevolucionMaterialVM tmp = AutoMapperGenericsHelper<tDevolucionMaterial, DevolucionMaterialVM>.ConvertToDBEntity(entity_db);

            entity.FB = tmp.FB;
            entity.UB = tmp.UB;

            entity.FG = tmp.FG;
            entity.UG = tmp.UG;
            entity.ST = tmp.ST;

            entity.FM = DateTime.Now;
            entity.UM = HttpContext.Current.User.Identity.GetUserId();

            tDevolucionMaterial convert = AutoMapperGenericsHelper<DevolucionMaterialVM, tDevolucionMaterial>.ConvertToDBEntity(entity);

            _UoW.Repository<tDevolucionMaterial>().Editar(convert);
        }

        public void Eliminar(object id)
        {

            //tDevolucionMaterial entity_db = new RepositoryGenerico<tDevolucionMaterial>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);
            tDevolucionMaterial entity_db = _UoW.Repository<tDevolucionMaterial>().ObtenerPorId(id);

            entity_db.FB = DateTime.Now;
            entity_db.UB = HttpContext.Current.User.Identity.GetUserId();
            entity_db.ST = false;

            _UoW.Repository<tDevolucionMaterial>().Eliminar(entity_db);
           
        }

        public List<MaterialVM> EncontrarPor(ParametrosQuery<MaterialVM> parametrosDeQuery)
        {
            List<MaterialVM> result = null;

            Expression<Func<MaterialVM, bool>> Where2 = w => w.ST == true;
            parametrosDeQuery.Where = PredicateBuilder.And(Where2, parametrosDeQuery.Where);

            ParametrosQuery<tDetalleMaterial> domainPredicate = AutoMapperGenericsHelper<ParametrosQuery<MaterialVM>, ParametrosQuery<tDetalleMaterial>>.ConvertToDBEntity(parametrosDeQuery);

            IEnumerable<tDetalleMaterial> entitys = _UoW.Repository<tDetalleMaterial>().EncontrarPor(domainPredicate.Where);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleMaterial>, List<MaterialVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public MaterialVM Obtener(Expression<Func<tDetalleMaterial, bool>> predicate)
        {
            MaterialVM result = null;

            tDetalleMaterial entity = _UoW.Repository<tDetalleMaterial>().Obtener(predicate);

            result = AutoMapperGenericsHelper<tDetalleMaterial, MaterialVM>.ConvertToDBEntity(entity);

            return result;
        }

        public DevolucionMaterialVM ObtenerPorId(object id)
        {
            DevolucionMaterialVM result = null;

            tDevolucionMaterial entity = _UoW.Repository<tDevolucionMaterial>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tDevolucionMaterial, DevolucionMaterialVM>.ConvertToDBEntity(entity);

            //using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            //{
            //    result.tSustanciaMaterial = db.ObtenerPorId(result.idSustanciaMaterial);
            //}

            AlmacenMaterialVM lote = new AlmacenMaterialVM();

            using (CndAlmacenDopajeEntities db = new CndAlmacenDopajeEntities())
            {
                if (result.idAlmacen > 0)
                {
                    lote = new LeerCatalogosServicio<AlmacenMaterialVM, tAlmacenMaterial>(db)
                            .EncontrarPor(w => w.idAlmacen == result.idAlmacen).FirstOrDefault();
                }
            }

            if (lote != null)
            {
                result.cCondicionesAlmacenaje = lote.cCondicionesAlmacenaje;
                result.cMarca = lote.cMarca;
                result.cPresentacion = lote.cPresentacion;
                result.cProveedor = lote.cProveedor;
            }
            return result;
        }

        public List<MaterialVM> ObtenerTodos(Expression<Func<tDetalleMaterial, bool>> predicate = null)
        {
            List<MaterialVM> result = null;

            predicate=predicate.And(w => w.ST == true && w.tDevolucion.Count > 0);

            IEnumerable<tDetalleMaterial> entitys = _UoW.Repository<tDetalleMaterial>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleMaterial>, List<MaterialVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public BaseListaCatalogosVM<MaterialVM>  ObtenerPaginado(ParametrosQuery<tDetalleMaterial> parametrosDeQuery)
        {
            BaseListaCatalogosVM<MaterialVM> result = new BaseListaCatalogosVM<MaterialVM>();

            if (parametrosDeQuery.Where != null)
            {
                parametrosDeQuery.Where = parametrosDeQuery.Where.And(w => w.ST == true && w.tDevolucion.Count > 0);
            }
            else
            {
                parametrosDeQuery.Where= w => w.ST == true && w.tDevolucion.Count > 0;
            }

            Paginacion<tDetalleMaterial> paginado = _UoW.Repository<tDetalleMaterial>().ObtenerPaginado(parametrosDeQuery);


            result.Paginacion = AutoMapperGenericsHelper<Paginacion<tDetalleMaterial>, Paginacion<MaterialVM>>.ConvertToDBEntity(paginado);

            result.Paginador = Mapper.Map<Paginacion<MaterialVM>, PaginadorViewModels>(result.Paginacion);

            return result;
        }

        public BaseListaAlmacenVM<MaterialVM, DevolucionMaterialVM> ObtenerExistencias(int idSustanciaMaterial, Expression<Func<tDevolucionMaterial, bool>> predicateExistencias = null)
        {
            BaseListaAlmacenVM<MaterialVM, DevolucionMaterialVM> result = new BaseListaAlmacenVM<MaterialVM, DevolucionMaterialVM>();

            predicateExistencias=predicateExistencias.And(w => w.ST == true);

            IEnumerable<tDevolucionMaterial> entitys = _UoW.Repository<tDevolucionMaterial>().ObtenerTodos(predicateExistencias);

            result.Lista = AutoMapperGenericsHelper<IEnumerable<tDevolucionMaterial>, List<DevolucionMaterialVM>>.ConvertToDBEntity(entitys);

            using (LeerCatalogosServicio<MaterialVM, tDetalleMaterial> db = new LeerCatalogosServicio<MaterialVM, tDetalleMaterial>())
            {
                result.Material = db.ObtenerPorId(idSustanciaMaterial);
            }

            return result;
        }

        public int Existencias(int idSustanciaMaterial, Expression<Func<tDevolucionMaterial, bool>> predicateExistencias = null)
        {
            int result = 0;

            predicateExistencias = predicateExistencias.And(w => w.ST == true);

            result = _UoW.Repository<tDevolucionMaterial>().EncontrarPor(predicateExistencias).Sum(su=>su.Existencia);

            return result;
        }
        private static void FiltrosYSoloActivos(Expression<Func<tDetalleMaterial, bool>> parametros)
        { 

            Expression<Func<MaterialVM, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;

            if (parametros != null)
            {
                var domainPredicate = AutoMapperGenericsHelper<Expression<Func<tDetalleMaterial, bool>>, Expression<Func<MaterialVM, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            parametros = AutoMapperGenericsHelper<Expression<Func<MaterialVM, bool>>, Expression<Func<tDetalleMaterial, bool>>>.ConvertToDBEntity(lambda);
        }

        public ResultSave Save()
        {
           return _UoW.SaveChanges();
        }

        public int GenNumDeLoteInterno(int idSustanciaMaterial)
        {
            int result = 1;

            tDevolucionMaterial material= _UoW.Repository<tDevolucionMaterial>()
                .EncontrarPor(w=> w.idSustanciaMaterial== idSustanciaMaterial)
                .OrderByDescending(o => o.NumDeLoteInterno).Take(1).FirstOrDefault();

            if (material!=null)
            {
                result = material.NumDeLoteInterno+1;
            }

            return result;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _UoW.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
    }

}