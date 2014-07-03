using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Filters;

namespace StreamingService
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            base.OnActionExecuted(actionExecutedContext);
        }
    }

    [AllowCrossSiteJson]
    public class StreamingController : ApiController
    {
        public class Staff
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private static List<Staff> _staffMembers = new List<Staff>
        {
            new Staff { Id = 0, FirstName = "Bob", LastName = "Smith" },
            new Staff { Id = 1, FirstName = "Bert", LastName = "Brown" },
            new Staff { Id = 2, FirstName = "Jill", LastName = "Jones" },
        };

        [HttpGet]
        [ActionName("staff")]
        public HttpResponseMessage Get(int id)
        {
            var staffMember = _staffMembers.FirstOrDefault(s => s.Id == id);

            if (staffMember == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, "Staff member not found!");
            }
            else
            {
                return Request.CreateResponse(staffMember);
            }
        }

        [HttpGet]
        [ActionName("image")]
        public HttpResponseMessage StreamContent(int id)
        {
            var staffMember = _staffMembers.FirstOrDefault(s => s.Id == id);

            if (staffMember == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, "Staff member not found!");
            }
            else
            {
                var response = Request.CreateResponse();

                // This method uses a local byte array, and incurs a Large Object Heap cost
                //byte[] fileData = File.ReadAllBytes(HostingEnvironment.MapPath(string.Format("~/images/{0}.jpg", staffMember.Id)));
                //response.Content = new ByteArrayContent(fileData);

                // This method streams file data direct to the HTTP response 
                response.Content = new StreamContent(
                    File.OpenRead(
                        HostingEnvironment.MapPath(string.Format("~/images/{0}.jpg", staffMember.Id))));

                response.Content.Headers.ContentType = 
                    new MediaTypeHeaderValue("image/jpeg");

                return response;
            }
        }

        [HttpGet]
        [ActionName("stafflist")]
        public HttpResponseMessage PushStreamContent_StaffList()
        {
            var response = Request.CreateResponse();

            response.Content = new PushStreamContent((stream, content, context) =>
            {
                foreach (var staffMember in _staffMembers)
                {
                    var serializer = new JsonSerializer();
                    using (var writer = new StreamWriter(stream))
                    {
                        serializer.Serialize(writer, staffMember);
                        stream.Flush();
                    }

                    Thread.Sleep(1000);
                }

                stream.Close();
            });

            return response;
        }

        [HttpPost]
        [ActionName("updateimage")]
        public async Task<HttpResponseMessage> ReadStream(int id)
        {
            var staffMember = _staffMembers.FirstOrDefault(s => s.Id == id);

            if (staffMember == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, "Staff member not found!");
            }
            else
            {
                using (var stream = await Request.Content.ReadAsStreamAsync())
                {
                    var fileStream = File.OpenWrite(
                        HostingEnvironment.MapPath(
                            string.Format("~/images/{0}.jpg", staffMember.Id)));

                    stream.CopyTo(fileStream);

                    fileStream.Close();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }
    }
}
