using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageSharing.Data;

namespace ImageSharingWithUsers.Models
{
    public class MyLikedViewModel
    {
        public IEnumerable<Image> MyLikedImages { get; set; }
    }
}