using System.Globalization;
using System.Xml.Linq;

class AddImageConfig
{
    // ---------------- Properties ----------------

    [FilePathArgument(
        "image_input",
        Description = "The image JPG image.",
        MustExist = true,
        Required = true
    )]
    public FilePath ImageInput { get; set; }

    [FilePathArgument(
        "image_metadata",
        Description = "The image's metadata file that came out of the generator.",
        MustExist = true,
        Required = true
    )]
    public FilePath ImageMetaData { get; set; }

    // ---------------- Functions ----------------

    public override string ToString()
    {
        return ArgumentBinder.ConfigToStringHelper( this );
    }
}

class ImageMetaData
{
    // ---------------- Fields ----------------

    public const string XmlElementName = "BettaImage";

    private static TextInfo enUs = new CultureInfo( "en-US", false ).TextInfo;

    // ---------------- Properties ----------------

    public string BaseImageName { get; set; }

    public string BaseImageWithoutExtension =>
        System.IO.Path.GetFileNameWithoutExtension( this.BaseImageName );

    public string OutputFileName { get; set; }

    public string BackgroundColor { get; set; }

    public string LineColor { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string Algorithm { get; set; }

    public DateTime CreationTime { get; set; }

    // ---------------- Functions ----------------

    private string GetBaseImageWithCasing()
    {
        string title = this.BaseImageWithoutExtension.Replace( '_', ' ' );
        return enUs.ToTitleCase( title );
    }

    public string GetTitle()
    {
        return $"{GetBaseImageWithCasing()} ({this.CreationTime.ToString( "yyyy-MM-dd" )})";
    }

    public string GetDescription()
    {
        string title = $"{GetBaseImageWithCasing()} with {this.LineColor} outline on a {this.BackgroundColor} background.";
        return enUs.ToTitleCase( title );
    }

    public string GetTags()
    {
        return $"[{GetBaseImageWithCasing()}, {this.LineColor} Outline, {this.BackgroundColor} Background, {this.Algorithm} Algorithm]";
    }
}

ImageMetaData ParseMetaData( FilePath path )
{
    XDocument doc = XDocument.Load( path.ToString() );

    var root = doc.Root;
    if( root is null )
    {
        throw new InvalidOperationException(
            "Unable to parse meta data file.  Root is null"
        );
    }
    else if( ImageMetaData.XmlElementName.Equals( root.Name.LocalName ) == false )
    {
        throw new ArgumentException(
            $"Invalid XML file, root node is '{root.Name.LocalName}', but expected '{ImageMetaData.XmlElementName}'."
        );
    }

    XElement metaDataNode = root.Elements().FirstOrDefault(
        e => "MetaData".Equals( e.Name.LocalName )
    );
    if( metaDataNode is null )
    {
        throw new InvalidOperationException(
            "Unable to parse meta data file, no MetaData node exist"
        );
    }

    var data = new ImageMetaData();

    foreach( XElement child in metaDataNode.Elements() )
    {
        string name = child.Name.LocalName;
        if( string.IsNullOrWhiteSpace( name ) )
        {
            continue;
        }
        else if( "BaseImage".Equals( name ) )
        {
            data.BaseImageName = child.Value;
        }
        else if( "OutputFileName".Equals( name ) )
        {
            data.OutputFileName = child.Value;
        }
        else if( "BackgroundColor".Equals( name ) )
        {
            data.BackgroundColor = child.Value;
        }
        else if( "LineColor".Equals( name ) )
        {
            data.LineColor = child.Value;
        }
        else if( "Width".Equals( name ) )
        {
            data.Width = int.Parse( child.Value );
        }
        else if( "Height".Equals( name ) )
        {
            data.Height = int.Parse( child.Value );
        }
        else if( "Algorithm".Equals( name ) )
        {
            data.Algorithm = child.Value;
        }
        else if( "CreationTime".Equals( name ) )
        {
            data.CreationTime = DateTimeOffset.ParseExact(
                child.Value,
                "O",
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind
            ).DateTime;
        }
    }

    return data;
}

Task( "add_image" )
.Does(
    () =>
    {
        var config = CreateFromArguments<AddImageConfig>();
        ImageMetaData data = ParseMetaData( config.ImageMetaData );

        string timeStamp = data.CreationTime.ToString( "yyyy_MM_dd" );

        string imagePath = $"static/bettas/{timeStamp}_{data.BaseImageName}";
        FilePath imageDestinationPath = File( imagePath );
        FilePath postPath = File( $"_posts/{timeStamp}_{data.BaseImageWithoutExtension}.md" );

        CopyFile( config.ImageInput, imageDestinationPath );
        CopyFile( config.ImageMetaData, File( $"_ImageMetaData/{timeStamp}_{data.BaseImageWithoutExtension}.xml" ) );

        string postContents =
$@"---
layout: betta
title: ""{data.GetTitle()}""
author: Bettadelic
comments: true
description: ""{data.GetDescription()}""
imageUrl: ""/{imagePath}""
tags: {data.GetTags()}
background_color: {data.BackgroundColor}
line_color: {data.LineColor}
width: {data.Width}
height: {data.Height}
algorithm: {data.Algorithm}
---
";

        System.IO.File.WriteAllText(
            postPath.ToString(),
            postContents
        );
    }
).DescriptionFromArguments<AddImageConfig>( "Adds an image to the site.");
