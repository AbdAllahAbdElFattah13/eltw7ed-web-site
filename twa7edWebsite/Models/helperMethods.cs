using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace twa7edWebsite.Models
{
    public class helperMethods
    {
        static public string formImageNameToImagePath(string imageName, string middleString)
        {
            var path = Path.Combine(HostingEnvironment.MapPath("~/Images/" + @middleString), imageName);
            return path;
        }

        private static string accessKey = "AKIAJBSKPAULFYKYYW3Q";
        private static string secretKey = "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";
 
        public static PutObjectResponse SaveImageToAmazon(String bucketName, HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);
            Amazon.S3.IAmazonS3 client;
            try
            {
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, RegionEndpoint.USWest2))
                {
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead,
                        Key = String.Format("{0}/{1}", bucketName, fileName),
                        InputStream = file.InputStream
                    };

                    return client.PutObject(request);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        public static DeleteObjectResponse DeleteImageFromAmazon(String bucketName, String ImageName)
        {
            try
            {
                IAmazonS3 client;
                client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

                DeleteObjectRequest deleteObjectRequest =
                    new DeleteObjectRequest
                    {
                        BucketName = bucketName,
                        Key = ImageName
                    };
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey))
                {
                    return client.DeleteObject(deleteObjectRequest);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static String ImageUrlFromBucketNameAndFileKey(String bucketName, String ImageName)
        {
            return String.Format("http://{0}.S3.amazonaws.com/{1}", bucketName, ImageName);
        }

    }

    public static class HttpPostedFileBaseExtensions
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

}