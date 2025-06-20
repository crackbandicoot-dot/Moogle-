﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace MoogleEngine.DocumentsUtils
{
    class PDFReader : IDocumentReader, IDisposable
    {
        private int currentPageNumber;
        private PdfDocument pdfDocument;
        private bool disposed = false;

        public PDFReader(string path)
        {
            pdfDocument = PdfDocument.Open(path);
            currentPageNumber = 1;
            DocumentPath = path;
        }

        public string DocumentPath { get; }

        public bool EndOfDocument()
        {
            return currentPageNumber > pdfDocument.NumberOfPages;
        }

        public string ReadPage()
        {
            if (EndOfDocument())
                return "";

            string pageContent = "";
            try
            {
                var page = pdfDocument.GetPage(currentPageNumber);
                pageContent = string.Join(" ", page.GetWords().Select(w => w.Text));

                // Remove numbers, symbols, and non-alphabetic characters
                pageContent = Regex.Replace(pageContent, @"[^\p{L}' -]", " ");
                // Collapse multiple spaces and trim
                pageContent = Regex.Replace(pageContent, @"\s+", " ").Trim();
            }
            catch (Exception)
            {
                // Fallback: Return empty string on failure
            }
            finally
            {
                currentPageNumber++;
            }
            return pageContent;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                pdfDocument?.Dispose();
            }
            disposed = true;
        }

        ~PDFReader()
        {
            Dispose(false);
        }
    }
}


