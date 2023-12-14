using WebApp.Models;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers.Image
{
    public class ImageController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Get(int id)
        {
            var image = _db.Image.FirstOrDefault(x => x.Id == id);
            if (image == null)
            {
                var bytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/Images/no-image.png"));
                return File(bytes, "image/png");
            }
            return File(image.ImageContent, "image/jpg");
        }

        public ActionResult GetByModel(string model)
        {
            int pid = _db.Product.FirstOrDefault(x => x.Model == model).Id;
            var image = _db.Selling.Include("Image1").FirstOrDefault(x => x.Product == pid).Image1;
            if (image == null)
            {
                var bytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/Images/no-image.png"));
                return File(bytes, "image/png");
            }

            return File(image.ImageContent, "image/jpg");
        }

        public ActionResult GetCached(int id)
        {
            var image = _db.Image.FirstOrDefault(x => x.Id == id);
            if (image == null)
            {
                var bytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/Images/no-image.png"));
                return File(bytes, "image/png");
            }

            return File(image.LowresImage, "image/jpg");
        }
    }
}