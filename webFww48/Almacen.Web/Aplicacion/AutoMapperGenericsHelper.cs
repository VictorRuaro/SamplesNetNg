using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Almacen.Web.ViewModels;
using Almacen.Data;

namespace Almacen.Web.Aplicacion
{
    public class AutoMapperGenericsHelper<TSource, TDestination>
    {
        public static TDestination ConvertToDBEntity(TSource model)
        {

            return Mapper.Map<TSource, TDestination>(model);
        }


    }
}