# BookNavigate
This purpose of this project is to gather printed word/char images as training data for machine learning task. 
* To gather word level data, google open book is scanned, then the original data can be synthetized. These three projects need to be run in proper order: **BookDownloader** - **BookImageProcessor** - **ImageSynthesizer**

* To gather single char level data, windows font images containing single char are generated, then the original data can be synthetized. These two projects need to be run in proper order: **CharGenerator** - **ImageSynthesizer**

## BookDownloader
Use selenium to navigate through google open books and download each book page as one image and save to harddisk. 

### Configuration

* Configure App.config - appsetting - BookDataPath, so the program knows where to save the images in the harddrive
* Configure Book.cs, so the program knows which books to navigate



## BookImageProcessor
extract word images from the images extracted from **BookDownloader** using OCR Service - Oxford

### Configuration

* Configure App.config - appsetting - BookDataPath, the path should point to the folder where the images are downloaded from 
**BookDownloader** 
* Configure App.config - appsetting - OxfordKey



## CharGenerator
extract single character images from various windows and google fonts. Windows fonts come with operating system, google fonts can be downloaded here, https://github.com/google/fonts

### Configuration

* Configure App.config - appsetting - ExportDir, Location there char images will be exported to 
* Configure App.config - appsetting - GeneratedChars, a complete string containing all chars that should be exported as images
* Configure App.config - appsetting - GoogleFontDir, after downloaded the complete google font project from git, the "ofl" folder
* Configure App.config - appsetting - DefaultExportFontSize, the default font size used to generate image









