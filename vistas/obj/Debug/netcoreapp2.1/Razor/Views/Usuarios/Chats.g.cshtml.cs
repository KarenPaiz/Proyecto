#pragma checksum "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "311b61ab94852242492f0dca5f3b301b246afc0f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Usuarios_Chats), @"mvc.1.0.view", @"/Views/Usuarios/Chats.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Usuarios/Chats.cshtml", typeof(AspNetCore.Views_Usuarios_Chats))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\_ViewImports.cshtml"
using vistas;

#line default
#line hidden
#line 2 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\_ViewImports.cshtml"
using vistas.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"311b61ab94852242492f0dca5f3b301b246afc0f", @"/Views/Usuarios/Chats.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"454f9e660b408d9dfe46b7353e87b98088dee59e", @"/Views/_ViewImports.cshtml")]
    public class Views_Usuarios_Chats : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
  
    ViewData["Title"] = "Chats";

#line default
#line hidden
            BeginContext(43, 82, true);
            WriteLiteral("\r\n<table style=\"width:100%\">\r\n\r\n    <tr>\r\n        <th>Mensajes</th>\r\n\r\n    </tr>\r\n");
            EndContext();
#line 12 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
     foreach (var item in ViewBag.Matriz)
    {

#line default
#line hidden
            BeginContext(175, 30, true);
            WriteLiteral("        <tr>\r\n            <td>");
            EndContext();
            BeginContext(206, 11, false);
#line 15 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
           Write(item.emisor);

#line default
#line hidden
            EndContext();
            BeginContext(217, 3, true);
            WriteLiteral(":  ");
            EndContext();
            BeginContext(221, 12, false);
#line 15 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
                          Write(item.mensaje);

#line default
#line hidden
            EndContext();
            BeginContext(233, 8, true);
            WriteLiteral("       -");
            EndContext();
            BeginContext(242, 9, false);
#line 15 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
                                               Write(item.hora);

#line default
#line hidden
            EndContext();
            BeginContext(251, 22, true);
            WriteLiteral("</td>\r\n        </tr>\r\n");
            EndContext();
#line 17 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
    }

#line default
#line hidden
            BeginContext(280, 59, true);
            WriteLiteral("</table>\r\n<h2></h2>\r\n<h2></h2>\r\n<div class=\"input-group\">\r\n");
            EndContext();
#line 22 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
     using (Html.BeginForm("Chats", "Usuarios", FormMethod.Post, new { enctype = "multipart/form-data" }))
    {

#line default
#line hidden
            BeginContext(454, 171, true);
            WriteLiteral("        <input type=\"text\" name=\"Mensaje\">\r\n        <input type=\"submit\" value=\"Enviar\" class=\"btn btn-default\" />\r\n        <input type=\"file\" name=\"ArchivoImportado\" />\r\n");
            EndContext();
#line 27 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
    }

#line default
#line hidden
            BeginContext(632, 83, true);
            WriteLiteral("</div>\r\n<h2></h2>\r\n<h2></h2>\r\n<h3>Buscar mensajes</h3>\r\n<div class=\"input-group\">\r\n");
            EndContext();
#line 33 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
     using (Html.BeginForm("Chats", "Usuarios", FormMethod.Post, new { enctype = "multipart/form-data" }))
    {

#line default
#line hidden
            BeginContext(830, 116, true);
            WriteLiteral("        <input type=\"text\" name=\"Mensaje\">\r\n        <input type=\"submit\" value=\"Enviar\" class=\"btn btn-default\" />\r\n");
            EndContext();
#line 37 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
    }

#line default
#line hidden
            BeginContext(953, 64, true);
            WriteLiteral("</div>\r\n<h3>Descargar archivos</h3>\r\n<div class=\"input-group\">\r\n");
            EndContext();
#line 41 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
     using (Html.BeginForm("Chats", "Usuarios", FormMethod.Post, new { enctype = "multipart/form-data" }))
    {

#line default
#line hidden
            BeginContext(1132, 116, true);
            WriteLiteral("        <input type=\"text\" name=\"Mensaje\">\r\n        <input type=\"submit\" value=\"Enviar\" class=\"btn btn-default\" />\r\n");
            EndContext();
#line 45 "C:\Users\Karen\Desktop\RamaInicial\Proyecto\vistas\Views\Usuarios\Chats.cshtml"
    }

#line default
#line hidden
            BeginContext(1255, 6, true);
            WriteLiteral("</div>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
