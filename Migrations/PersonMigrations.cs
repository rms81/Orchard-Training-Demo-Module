using System;
using Lombiq.TrainingDemo.Indexes;
using Lombiq.TrainingDemo.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace Lombiq.TrainingDemo.Migrations
{
    // Here's another migrations file but specifically for Person-related operations. Don't forget to register this
    // class to the service provider (see: Startup.cs).
    public class PersonMigrations : DataMigration
    {
        IContentDefinitionManager _contentDefinitionManager;


        public PersonMigrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }


        public int Create()
        {
            // Now you can configure PersonPart. For example you can add content fields (as mentioned earlier) here.
            _contentDefinitionManager.AlterPartDefinition(nameof(PersonPart), part => part
                // Each field has its own configuration. Here you will give a display name for it and add some
                // additional settings like a hint to be displayed in the editor.
                .WithField(nameof(PersonPart.Biography), field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Biography")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Person's biography"
                    })
                    .WithSettings(new ContentPartFieldSettings
                    {
                        // This is an extension point for providing multiple editor flavors for a content field or
                        // content part. We'll learn this feature later with the content fields.
                        Editor = "TextArea"
                    })));

            // We create a new content type. Note that there's only an alter method: this will create the type if it
            // doesn't exist or modify it if it does. Make sure you understand what content types are:
            // http://docs.orchardproject.net/Documentation/Content-types. The content type's name is arbitrary, but
            // choose a meaningful one. Notice that we attach parts by specifying their name. For our own parts we use
            // nameof(): this is not mandatory but serves great if we change the part's name during development.
            _contentDefinitionManager.AlterTypeDefinition("Person", builder => builder
                .Creatable()
                .Listable()
                .WithPart(nameof(PersonPart))
            );

            // This one will create an index table for the PersonPartIndex as explained in the BookMigrations file.
            SchemaBuilder.CreateMapIndexTable(nameof(PersonPartIndex), table => table
                .Column<DateTime>(nameof(PersonPartIndex.BirthDateUtc))
                .Column<string>(nameof(PersonPartIndex.ContentItemId), c => c.WithLength(26))
            );

            return 1;
        }
    }
}

// NEXT STATION: Indexes/PersonPartIndex