using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UltimateRedditBot.App.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseUltimateExceptionHandler(this IApplicationBuilder application)
        {
            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception is null)
                        return Task.CompletedTask;

                    try
                    {
                        
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                    }

                    return Task.CompletedTask;
                });
            });
        }
    }
}
