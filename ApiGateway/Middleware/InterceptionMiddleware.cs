namespace ApiGateway.Middleware
{
    public class InterceptionMiddleware(RequestDelegate next)
    { //this function is used to add a header to the request
      public async Task InvokeAsync(HttpContext context) { 
            context.Request.Headers["Referer"] = "Api-Gateway"; //we check at each request if it has header Referrer then it passed through gateway
            await next(context); 
      } 
    }
}