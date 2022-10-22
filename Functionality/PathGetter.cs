using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Functionality
{
    public static class PathGetter
    {
        public static readonly string[] supportedExtensions = new string[] { "JPG", "JPEG", "PNG" };

        public static List<string> GetImagePathsFromFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "You must select a folder containing images"
            };

            if (DialogResult.OK != folderBrowserDialog.ShowDialog())
            {
                return null;
            }

            List<string> paths = new List<string>();
            paths.AddRange
                (
                    FilterFiles(Directory.GetFiles(folderBrowserDialog.SelectedPath))
                );
            return ShufflePaths(paths);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multipleNNs">If multiple nn is set to true combo box will be filled with options</param>
        public static List<string> GetImagePathsFromFolderContainingImageFolders(bool multipleNNs)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "You must select a folder with folders that contains images"
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return null;
            }


            List<string> directories = new List<string>(Directory.GetDirectories(folderBrowserDialog.SelectedPath));

            List<string[]> imagesPaths = new List<string[]>();
            List<string> unhirearchicalImagePaths = new List<string>();

            foreach (var imageDirectory in directories)
            {
                string[] currentPaths;
                imagesPaths.Add(currentPaths = FilterFiles(Directory.GetFiles(imageDirectory)));
                unhirearchicalImagePaths.AddRange(currentPaths);
            }

            return ShufflePaths(unhirearchicalImagePaths);
        }

        public static string GetImagePath()
        {
            string output = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.BMP, *.JPG, *.PNG)|*.BMP;*.JPG;*.PNG";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Select image";

                bool isSupportedFile = false;
                while (!isSupportedFile)
                {
                    if (DialogResult.Cancel == openFileDialog.ShowDialog())
                    {
                        return null;
                    }
                    output = openFileDialog.FileName;
                    isSupportedFile = IsSupportedFile(output);
                    if (!isSupportedFile)
                        MessageBox.Show("Please select an image");
                }
            }
            return output;
        }

        public static bool IsSupportedFile(string filePath)
        {
            bool containsSupportedExtension = false;
            foreach (var supportedExtension in supportedExtensions)
                containsSupportedExtension = filePath.ToLowerInvariant().Contains(supportedExtension.ToLowerInvariant()) || containsSupportedExtension;
            return containsSupportedExtension;
        }

        public static string[] FilterFiles(string[] filePaths)
        {
            var output = new List<string>();
            foreach (var filePath in filePaths)
            {
                if (IsSupportedFile(filePath))
                    output.Add(filePath);
            }
            return output.ToArray();
        }

        public static int rI = 0;
        public static List<string> ShufflePaths(List<string> paths)
        {
            //Create a copy
            List<string> input = paths.ToList();
            Random r = new Random(DateTime.Now.Millisecond + rI++);

            List<string> output = new List<string>();
            int pathsCount = paths.Count;
            for (int i = 0; i < pathsCount; i++)
            {
                int selectedI = r.Next(input.Count);

                output.Add(input[selectedI]);
                input.RemoveAt(selectedI);
            }
            return output;
        }
    }
}
