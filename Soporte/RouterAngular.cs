using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class RouterAngular
    {
        private Tabla[] tablas;
        public RouterAngular(Tabla[] tablas) {
            this.tablas = tablas;
        }

        private string name(Tabla tabla)
        {
            return tabla.name.Substring(0, 1).ToUpper() + ((tabla.name != null && tabla.name.Length > 1) ? tabla.name.Substring(1, tabla.name.Length - 1) : "");
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res .Append(@"
import { NgModule, Injectable } from '@angular/core';
import { RouterModule, Routes, DefaultUrlSerializer, UrlSerializer, UrlTree, TitleStrategy } from '@angular/router';

");
            foreach (Tabla t in tablas) res.Append(@"import { " + name(t) + @"Component } from './components/" + t.name.ToLower() + @"/" + t.name.ToLower() + @".component';
");

            res.Append(@"
export const routes: Routes = [
");
            foreach (Tabla t in tablas) res.Append(@"{ path: '" + t.name.ToLower() + @"', component: " + name(t) + @"Component, title: '" + name(t)  + @"' },
");

            res.Append(@"
];
");

            return res.ToString();
        }
    }
}
