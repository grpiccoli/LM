using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CLM.Extensions
{
    public static class EViewData
    {
        public static MultiSelectList Enum2Select<TEnum>(this TEnum e,
            IDictionary<string, List<string>> Filters = null, string name = null)
        {
            var tipo = typeof(TEnum);
            var nombre = tipo.ToString().Split('.').Last();
            switch (name)
            {
                case "Description":
                    {
                        return new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Id = s, Name = s.GetAttrDescription() },
                            "Id", "Name",
                            Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null);
                    }
                case "Name":
                    {
                        return new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Id = s, Name = s.GetAttrName() },
                            "Id", "Name",
                            Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null);
                    }
                default:
                    {
                        return new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Id = s, Name = s.ToString() },
                            "Id", "Name",
                            Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null);
                    }
            }
        }

        public static string GetAttrDescription<TEnum>(this TEnum e)
        {
            return ((DisplayAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false))[0].Description;
        }

        public static string GetAttrName<TEnum>(this TEnum e)
        {
            return ((DisplayAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false))[0].Name;
        }

        public static string GetAttrGroupName<TEnum>(this TEnum e)
        {
            return ((DisplayAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false))[0].GroupName;
        }
    }
}
