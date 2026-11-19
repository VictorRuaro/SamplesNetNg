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
using Almacen.Web.Tools;

namespace Almacen.Web.Applicacion
{
    public class SustanciasServicio : IDisposable
    {
        private readonly IGenericUnitOfWork _UoW;

        public SustanciasServicio(DbContext entities)
        {
            _UoW = new GenericUnitOfWork(entities);

        }
        public SustanciasServicio(IGenericUnitOfWork UoW)
        {
            _UoW = UoW;
        }

        public AlmacenSustanciaVM Agregar(AlmacenSustanciaVM entity)
        {
            AlmacenSustanciaVM result = null;

            entity.FG = DateTime.Now;
            entity.UG= HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            entity.CantidadIngresa = entity.Existencia;

            tAlmacenSustancia convert = AutoMapperGenericsHelper<AlmacenSustanciaVM, tAlmacenSustancia>.ConvertToDBEntity(entity);

            

            tAlmacenMovimiento mov = new tAlmacenMovimiento();

            mov.Cantidad = convert.CantidadIngresa;
            mov.idAlmacenTipoMovimiento = (int)EnumAlmacenMovimiento.Entrada;

            mov.FG = DateTime.Now;
            mov.UG = HttpContext.Current.User.Identity.GetUserId();
            mov.ST = true;

            convert.tAlmacenMovimiento.Add(mov);

            //_UoW.Repository<tAlmacenMovimiento>().Agregar(mov);

            _UoW.Repository<tAlmacenSustancia>().Agregar(convert);

            result = AutoMapperGenericsHelper<tAlmacenSustancia, AlmacenSustanciaVM>.ConvertToDBEntity(convert);

            return result;
        }

        public int Contar(Expression<Func<SustanciaVM, bool>> where)
        {
            int result = 0;

            //comentar en caso de obtener tambien los registros inactivos
            Expression<Func<SustanciaVM, bool>> Where2 = w => w.ST == true;
            where = PredicateBuilder.And(Where2, where);

            Expression<Func<tDetalleSustacia, bool>> domainPredicate = AutoMapperGenericsHelper<Expression<Func<SustanciaVM, bool>>, Expression<Func<tDetalleSustacia, bool>>>.ConvertToDBEntity(where);

            _UoW.Repository<tDetalleSustacia>().Contar(domainPredicate);

            return result;
        }

        public void Editar(AlmacenSustanciaVM entity,int id)
        {
            tAlmacenSustancia tmp=null;
            using (CndAlmacenDopajeEntities bd=new CndAlmacenDopajeEntities())
            {
                tmp = new RepositoryGenerico<tAlmacenSustancia>(bd).ObtenerPorId(id);
            }
                
            //AlmacenSustanciaVM tmp = AutoMapperGenericsHelper<tAlmacenSustancia, AlmacenSustanciaVM>.ConvertToDBEntity(entity_db);

            entity.FB = tmp.FB;
            entity.UB = tmp.UB;

            entity.FG = tmp.FG;
            entity.UG = tmp.UG;
            entity.ST = tmp.ST;

            entity.FM = DateTime.Now;
            entity.UM = HttpContext.Current.User.Identity.GetUserId();

            tAlmacenSustancia convert = AutoMapperGenericsHelper<AlmacenSustanciaVM, tAlmacenSustancia>.ConvertToDBEntity(entity);


            _UoW.Repository<tAlmacenSustancia>().Editar(convert);
        }

        public void Eliminar(object id)
        {

            //tAlmacenSustancia entity_db = new RepositoryGenerico<tAlmacenSustancia>(new CndAlmacenDopajeEntities()).ObtenerPorId(id);
            tAlmacenSustancia entity_db=_UoW.Repository<tAlmacenSustancia>().ObtenerPorId(id);

            entity_db.FB = DateTime.Now;
            entity_db.UB = HttpContext.Current.User.Identity.GetUserId();
            entity_db.ST = false;
            
            _UoW.Repository<tAlmacenSustancia>().Eliminar(entity_db);
           
        }

        public List<SustanciaVM> EncontrarPor(ParametrosQuery<SustanciaVM> parametrosDeQuery)
        {
            List<SustanciaVM> result = null;

            Expression<Func<SustanciaVM, bool>> Where2 = w => w.ST == true;
            parametrosDeQuery.Where = PredicateBuilder.And(Where2, parametrosDeQuery.Where);

            ParametrosQuery<tDetalleSustacia> domainPredicate = AutoMapperGenericsHelper<ParametrosQuery<SustanciaVM>, ParametrosQuery<tDetalleSustacia>>.ConvertToDBEntity(parametrosDeQuery);

            IEnumerable<tDetalleSustacia> entitys = _UoW.Repository<tDetalleSustacia>().EncontrarPor(domainPredicate.Where);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleSustacia>, List<SustanciaVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public SustanciaVM Obtener(Expression<Func<tDetalleSustacia, bool>> predicate)
        {
            SustanciaVM result = null;

            tDetalleSustacia entity = _UoW.Repository<tDetalleSustacia>().Obtener(predicate);

            result = AutoMapperGenericsHelper<tDetalleSustacia, SustanciaVM>.ConvertToDBEntity(entity);

            return result;
        }

        public AlmacenSustanciaVM ObtenerPorId(object id)
        {
            AlmacenSustanciaVM result = null;

            tAlmacenSustancia entity = _UoW.Repository<tAlmacenSustancia>().ObtenerPorId(id);

            result = AutoMapperGenericsHelper<tAlmacenSustancia, AlmacenSustanciaVM>.ConvertToDBEntity(entity);

            //using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
            //{
            //    result.tSustanciaMaterial = db.ObtenerPorId(result.idSustanciaMaterial);
            //}

            return result;
        }

        public List<SustanciaVM> ObtenerTodos(Expression<Func<tDetalleSustacia, bool>> predicate = null)
        {
            List<SustanciaVM> result = null;

            predicate=predicate.And(w => w.ST == true 
            && w.tAlmacen.Count > 0
            && w.tAlmacen.Where(a => a.ST == true).Any());

            IEnumerable<tDetalleSustacia> entitys = _UoW.Repository<tDetalleSustacia>().ObtenerTodos(predicate);

            result = AutoMapperGenericsHelper<IEnumerable<tDetalleSustacia>, List<SustanciaVM>>.ConvertToDBEntity(entitys);

            return result;
        }

        public BaseListaCatalogosVM<SustanciaVM>  ObtenerPaginado(ParametrosQuery<tDetalleSustacia> parametrosDeQuery)
        {
            BaseListaCatalogosVM<SustanciaVM> result = new BaseListaCatalogosVM<SustanciaVM>();

            if (parametrosDeQuery.Where != null)
            {
                
                parametrosDeQuery.Where = parametrosDeQuery.Where.And(w => w.ST == true 
                                                                    && w.tAlmacen.Count > 0 
                                                                    && w.tAlmacen.Where(a=> a.ST==true).Any()
                                                                    );
            }
            else
            {
                parametrosDeQuery.Where= w => w.ST == true 
                && w.tAlmacen.Count > 0
                && w.tAlmacen.Where(a => a.ST == true).Any();
            }

            Paginacion<tDetalleSustacia> paginado = _UoW.Repository<tDetalleSustacia>().ObtenerPaginado(parametrosDeQuery);


            result.Paginacion = AutoMapperGenericsHelper<Paginacion<tDetalleSustacia>, Paginacion<SustanciaVM>>.ConvertToDBEntity(paginado);

            result.Paginador = Mapper.Map<Paginacion<SustanciaVM>, PaginadorViewModels>(result.Paginacion);

            return result;
        }

        public BaseListaAlmacenVM<SustanciaVM,AlmacenSustanciaVM> ObtenerExistencias(int idSustanciaMaterial, Expression<Func<tAlmacenSustancia, bool>> predicateExistencias = null)
        {
            BaseListaAlmacenVM<SustanciaVM, AlmacenSustanciaVM> result = new BaseListaAlmacenVM<SustanciaVM, AlmacenSustanciaVM>();

            predicateExistencias=predicateExistencias.And(w => w.ST == true);

            IEnumerable<tAlmacenSustancia> entitys = _UoW.Repository<tAlmacenSustancia>().ObtenerTodos(predicateExistencias);

            result.Lista = AutoMapperGenericsHelper<IEnumerable<tAlmacenSustancia>, List<AlmacenSustanciaVM>>.ConvertToDBEntity(entitys);

            using (LeerCatalogosServicio<SustanciaVM, tDetalleSustacia> db = new LeerCatalogosServicio<SustanciaVM, tDetalleSustacia>())
            {
                result.Material = db.ObtenerPorId(idSustanciaMaterial);
            }

            return result;
        }
        public int Existencias(int idSustanciaMaterial, Expression<Func<tAlmacenSustancia, bool>> predicateExistencias = null)
        {
            int result = 0;

            predicateExistencias = predicateExistencias.And(w => w.ST == true);

            result = _UoW.Repository<tAlmacenSustancia>().EncontrarPor(predicateExistencias).Sum(su => su.Existencia);

            return result;
        }
        private static void FiltrosYSoloActivos(Expression<Func<tDetalleSustacia, bool>> parametros)
        { 

            Expression<Func<SustanciaVM, bool>> Where2 = w => w.ST == true;

            var lambda = Where2;

            if (parametros != null)
            {
                var domainPredicate = AutoMapperGenericsHelper<Expression<Func<tDetalleSustacia, bool>>, Expression<Func<SustanciaVM, bool>>>.ConvertToDBEntity(parametros);
                lambda = PredicateBuilder.And(Where2, domainPredicate);
            }

            parametros = AutoMapperGenericsHelper<Expression<Func<SustanciaVM, bool>>, Expression<Func<tDetalleSustacia, bool>>>.ConvertToDBEntity(lambda);
        }

        public ResultSave Save()
        {
           return _UoW.SaveChanges();
        }

        public int GenNumDeLoteInterno(int idSustanciaMaterial)
        {
            int result = 1;

            tAlmacenSustancia material= _UoW.Repository<tAlmacenSustancia>()
                .EncontrarPor(w=> w.idSustanciaMaterial== idSustanciaMaterial)
                .OrderByDescending(o => o.NumDeLoteInterno).Take(1).FirstOrDefault();

            if (material!=null)
            {
                result = material.NumDeLoteInterno+1;
            }

            return result;
        }
        public AlmacenCertificadoVM AgregarCertificado(int idAlmacen, AlmacenCertificadoVM model)
        {

            tAlmacenCertificado entity = new tAlmacenCertificado();

            entity.idAlmacen = idAlmacen;
            entity.NombreArchivo = CustomExtension.GetFileName(model.Upload.FileName);
            entity.Archivo = CustomExtension.ConvertHttpFileToByte(model.Upload);
            entity.FG = DateTime.Now;
            entity.UG = HttpContext.Current.User.Identity.GetUserId();
            entity.ST = true;

            tAlmacenCertificado cert = _UoW.Repository<tAlmacenCertificado>().Agregar(entity);

            tAlmacenSustancia entity_db = _UoW.Repository<tAlmacenSustancia>().ObtenerPorId(idAlmacen);

            entity_db.FechaCaducidad = model.FechaCaducidad;

            _UoW.Repository<tAlmacenSustancia>().Editar(entity_db);

            model = AutoMapperGenericsHelper<tAlmacenCertificado, AlmacenCertificadoVM>.ConvertToDBEntity(cert);

            return model;

        }

        public AlmacenCertificadoVM ObtenerCertificado(int idAlmacenCertificado)
        {
            AlmacenCertificadoVM result = new AlmacenCertificadoVM();

            using (LeerCatalogosServicio<AlmacenCertificadoVM, tAlmacenCertificado> db = new LeerCatalogosServicio<AlmacenCertificadoVM, tAlmacenCertificado>())
            {
                result = db.ObtenerPorId(idAlmacenCertificado);
            }

            return result;

        }

        public void EliminarCertificado(int idAlmacenCertificado)
        {

            tAlmacenCertificado cert = _UoW.Repository<tAlmacenCertificado>().ObtenerPorId(idAlmacenCertificado);

            cert.FB = DateTime.Now;
            cert.UB = HttpContext.Current.User.Identity.GetUserId();
            cert.ST = false;

            _UoW.Repository<tAlmacenCertificado>().Eliminar(cert);

        }

        //private static Expression<Func<tDetalleSustacia, TType>> MapDomainModelExpression<TType>(Expression<Func<SustanciaVM, TType>> expression)
        //{
        //    return Mapper.Map<Expression<Func<SustanciaVM, TType>>, Expression<Func<tDetalleSustacia, TType>>>(expression);
        //}

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