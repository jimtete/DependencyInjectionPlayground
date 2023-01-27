using System.ComponentModel.Composition;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;

namespace DI_Playground
{
    [MetadataAttribute]
    public class AgeMetadataAttribute : Attribute
    {
        public int Age { get; set; }

        public AgeMetadataAttribute(int age)
        {
            Age = age;
        }
    }

    public interface IArtwork
    {
        void Display();
    }

    [AgeMetadata(100)]
    public class CenturyArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a century-old piece");
        }
    }

    [AgeMetadata(1000)]
    public class MillenialArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a really old piece of art");
        }
    }

    public class ArtDisplay
    {
        private IArtwork _artwork;

        public ArtDisplay([MetadataFilter("Age", 100)] IArtwork artwork)
        {
            _artwork = artwork;
        }

        public void Display()
        {
            _artwork.Display();
        }
    }
    
    public class Program
    {
        static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterModule<AttributedMetadataModule>();
            b.RegisterType<MillenialArtwork>().As<IArtwork>();
            b.RegisterType<CenturyArtwork>().As<IArtwork>();
            b.RegisterType<ArtDisplay>().WithAttributeFiltering();
            using (var c = b.Build())
            {
                c.Resolve<ArtDisplay>().Display();
            }
        }
    }
}