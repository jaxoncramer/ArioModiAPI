//
//  ContextGenerator.c
//  
//
//  Created by Jaxon Cramer on 10/5/17.
//

/*
 TODO:
 Get rid of Get all tag
 */

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>

#define MAX_NAME_STRING 50
#define MAX_CALL_STRING 100
#define MAX_FILE_STRING 150

#define MAX_STRINGS 10
#define MAX_INTS 10
#define MAX_DECIMALS 10

/* Prototypes */
void getCommandLine(int argc, char **argv);
void fileAccess(char *fileName, FILE **file_ptr);
void writeModelFile();
void writeDisplayModel();
void writeContextFile();
void writeInterface();
void writeRepository();
void writeController();
void cleanup();

/* Global variables for command line parameters */
char *tableName;
char *APICallName;

char **strings;
char **ints;
char **decimals;

int stringIndex = 0;
int intIndex = 0;
int decIndex = 0;

int main(int argc, char **argv) {
    
    tableName = (char *) malloc(MAX_NAME_STRING * sizeof(char));
    APICallName = (char *) malloc(MAX_CALL_STRING * sizeof(char));
    
    strings = (char **) malloc(MAX_STRINGS * sizeof(char *));
    ints = (char **) malloc(MAX_INTS * sizeof(char *));
    decimals = (char **) malloc(MAX_DECIMALS * sizeof(char *));
    
    getCommandLine(argc, argv);
    
    if (tableName[0] == '\0') {
        printf("Please enter a table name using the -t option.\n");
        exit(1);
    } else {
        
        if(APICallName[0] == '\0') {
            strcpy(APICallName, tableName);
        }
        
        writeModelFile();
        writeDisplayModel();
        writeContextFile();
        writeInterface();
        writeRepository();
        writeController();
    }
    
    //cleanup();
    
    return 0;
}

void writeModelFile() {
    FILE *file_ptr = NULL;
    char *fileName = (char *) malloc(MAX_FILE_STRING * sizeof(char));
    strcpy(fileName, "Ario.API/Models/");
    strcat(fileName, tableName);
    strcat(fileName, ".cs");
    
    fileAccess(fileName, &file_ptr);
    
    if(file_ptr != NULL) {
        const char *string1 = "namespace Ario.API.Models\n{\n\tpublic class ";
        fprintf(file_ptr, "%s%s%s", string1, tableName, "\n\t{\n");
        
        char *tempString;
        int i = 0;
        while(i < intIndex) {
            tempString = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString, "\t\tpublic int? %s { get; set; }\n", ints[i]);
            fprintf(file_ptr, "%s", tempString);
            free(tempString);
            i++;
        }
        i = 0;
        while(i < stringIndex) {
            tempString = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString, "\t\tpublic string %s { get; set; }\n", strings[i]);
            fprintf(file_ptr, "%s", tempString);
            free(tempString);
            i++;
        }
        i = 0;
        while(i < decIndex) {
            tempString = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString, "\t\tpublic decimal? %s { get; set; }\n", decimals[i]);
            fprintf(file_ptr, "%s", tempString);
            free(tempString);
            i++;
        }
        fprintf(file_ptr, "\t}\n}\n");
        
        fclose(file_ptr);
        free(fileName);
    }
}

void writeDisplayModel() {
    FILE *file_ptr = NULL;
    char *fileName = (char *) malloc(MAX_FILE_STRING * sizeof(char));
    strcpy(fileName, "Ario.API/Models/DisplayModels/");
    strcat(fileName, APICallName);
    strcat(fileName, "Display.cs");
    
    fileAccess(fileName, &file_ptr);
    
    if(file_ptr != NULL) {
    
        const char *string1 = "namespace Ario.API.Models.DisplayModels\n{\n\tpublic class ";
        const char *string2 = "Display\n\t{\n";
        fprintf(file_ptr, "%s%s%s", string1, APICallName, string2);
        
        char *tempString;
        int i = 0;
        while(i < intIndex) {
            tempString = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString, "\t\tpublic int? %s { get; set; }\n", ints[i]);
            fprintf(file_ptr, "%s", tempString);
            free(tempString);
            i++;
        }
        i = 0;
        while(i < stringIndex) {
            tempString = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString, "\t\tpublic string %s { get; set; }\n", strings[i]);
            fprintf(file_ptr, "%s", tempString);
            free(tempString);
            i++;
        }
        i = 0;
        while(i < decIndex) {
            tempString = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString, "\t\tpublic decimal? %s { get; set; }\n", decimals[i]);
            fprintf(file_ptr, "%s", tempString);
            free(tempString);
            i++;
        }
        fprintf(file_ptr, "\t}\n}\n");
        
        fclose(file_ptr);
        free(fileName);
    }
    
}

void writeContextFile() {
    FILE *file_ptr = NULL;
    char *fileName = (char *) malloc(MAX_FILE_STRING * sizeof(char));
    strcpy(fileName, "Ario.API/Contexts/");
    strcat(fileName, tableName);
    strcat(fileName, "Context.cs");
    
    fileAccess(fileName, &file_ptr);
    
    if(file_ptr != NULL) {
        char *strContext = (char *) malloc(MAX_FILE_STRING * sizeof(char));
        strcpy(strContext, tableName);
        strcat(strContext, "Context");
        
        const char *string1 = "using Ario.API.Models;\nusing Microsoft.EntityFrameworkCore;\n\n";
        const char *string2 = "namespace Ario.API.Contexts\n{\n\tpublic class ";
        const char *string3 = " : DbContext\n\t{\n\t\tpublic ";
        const char *string4 = "(DbContextOptions<";
        const char *string5 = "> options)\n\t\t\t: base(options) { }\n\n\t\tprotected override void OnModelCreating(ModelBuilder modelBuilder)\n\t\t{\n\t\t\tmodelBuilder.Entity<";
        const char *string6 = ">().HasKey(x => x.ID);\n\t\t\tbase.OnModelCreating(modelBuilder);\n\t\t}\n\n\t\tpublic DbSet<";
        const char *string7 = " { get; set; }\n\t}\n}";
        
        fprintf(file_ptr, "%s%s%s%s%s%s%s%s%s%s%s%s%s%s\n", string1, string2, strContext,
               string3, strContext, string4, strContext, string5, tableName,
               string6, tableName, "> ", tableName, string7);
        
        fclose(file_ptr);
        free(fileName);
        free(strContext);
    }
}

void writeInterface() {
    FILE *file_ptr = NULL;
    char *fileName = (char *) malloc(MAX_FILE_STRING * sizeof(char));
    strcpy(fileName, "Ario.API/Repositories/Interfaces/");
    strcat(fileName, "I");
    strcat(fileName, APICallName);
    strcat(fileName, "Repository.cs");
    
    fileAccess(fileName, &file_ptr);
    
    if(file_ptr != NULL) {
        const char *string1 = "using System.Collections.Generic;\nusing Ario.API.Models;\nusing Ario.API.Models.DisplayModels;\n\nnamespace Ario.API.Repositories\n{\n\tpublic interface I";
        const char *string2 = "Repository\n\t{\n\t\tvoid Add(";
        const char *string3 = " item);\n\t\tIEnumerable<";
        const char *string4 = "> GetAll();\n\t\tIEnumerable<";
        const char *string5 = "Display> GetAll(";
        const char *string6 = " Find(int id);\n\t\tvoid Remove(int id);\n\t\tvoid Update(";
        const char *string7 = " item);\n\t}\n}";
        
        fprintf(file_ptr,"%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s\n", string1, APICallName, string2,
               tableName, string3, tableName, string4, APICallName, string5,
               tableName, " item);\n\t\t", tableName, string6, tableName, string7);
        
        fclose(file_ptr);
        free(fileName);
    }
}

void writeRepository() {
    FILE *file_ptr = NULL;
    char *fileName = (char *) malloc(MAX_FILE_STRING * sizeof(char));
    strcpy(fileName, "Ario.API/Repositories/");
    strcat(fileName, APICallName);
    strcat(fileName, "Repository.cs");
    
    fileAccess(fileName, &file_ptr);
    
    if(file_ptr != NULL) {
        const char *string1 = "using System.Collections.Generic;\nusing System.Linq;\nusing Ario.API.Models;\nusing Ario.API.Contexts;\nusing Ario.API.Models.DisplayModels;\nusing System;\nusing System.Reflection;\nusing Ario.API.Attributes;\n\nnamespace Ario.API.Repositories\n{\n\tpublic class ";
        const char *string2 = "Repository : I";
        const char *string3 = "Repository\n\t{\n\t\t";
        const char *string4 = "Context _context;\n\t\tpublic ";
        const char *string5 = "Repository(";
        const char *string6 = "Context context)\n\t\t{\n\t\t\t_context = context;\n\t\t}\n\n\t\tpublic void Add(";
        const char *string7 = " item)\n\t\t{\n\t\t\t_context.";
        const char *string8 = ".Add(item);\n\t\t\t_context.SaveChanges();\n\t\t}\n\n\t\tpublic ";
        const char *string9 = " Find(int id)\n\t\t{\n\t\t\treturn _context.";
        const char *string10 = ".Where(e => e.ID == id).SingleOrDefault();\n\t\t}\n\n\t\tpublic IEnumerable<";
        const char *string11 = "> GetAll()\n\t\t{\n\t\t\treturn _context.";
        const char *string12 = ".ToList();\n\t\t}\n\n\t\tpublic IEnumerable<";
        const char *string13 = "Display> GetAll(";
        const char *string14 = " item)\n\t\t{\n\n\t\t\tList<";
        const char *string15 = "Display> displayList = new List<";
        const char *string16 = "Display>();\n\n\t\t\tif (item != null)\n\t\t\t{\n\t\t\t\tList<";
        const char *string17 = "> tempList =\n\t\t\t\t\t_context.";
        const char *string18 = ".ToList();\n\t\t\t\tList<";
        const char *string02 = "> list = new List<";
        const char *string03 = ">();\n\n\t\t\t\tforeach(";
        const char *string04 = " entry in tempList) {\n\t\t\t\t\tbool match = true;\n\t\t\t\t\tType type = entry.GetType();\n\t\t\t\t\tforeach (PropertyInfo propertyInfo in type.GetProperties())\n\t\t\t\t\t{\n\t\t\t\t\t\tif (propertyInfo.CanRead)\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\tif (propertyInfo.GetValue(item) != null && !propertyInfo.GetValue(item).ToString().Equals(\"0\"))\n\t\t\t\t\t\t\t{if(Attribute.IsDefined(propertyInfo, typeof(BlackListed))) {\n\t\t\t\t\t\t\t\t\tmatch = false;\n\t\t\t\t\t\t\t\t\tbreak;\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t\tif(propertyInfo.GetValue(entry) == null) {\n\t\t\t\t\t\t\t\t\tmatch = false;\n\t\t\t\t\t\t\t\t\tbreak;\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t\tif (!propertyInfo.GetValue(entry).Equals(propertyInfo.GetValue(item)))\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\tmatch = false;\n\t\t\t\t\t\t\t\t\tbreak;\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t}\n\t\t\t\t\t\t}\n\t\t\t\t\t}\n\t\t\t\t\tif(match) {\n\t\t\t\t\t\tlist.Add(entry);\n\t\t\t\t\t}\n\t\t\t\t}\n\n\t\t\t\tforeach (";
        const char *string19 = " t in list)\n\t\t\t\t{\n\t\t\t\t\t";
        const char *string20 = "Display disp = new ";
        const char *string21 = "Display();\n";
        const char *string01 = "\t\t\t\t\tdisplayList.Add(disp);\n\t\t\t\t}\n\t\t\t}\n\n\t\t\treturn displayList;\n\t\t}\n\n\t\tpublic void Remove(int id)\n\t\t{\n\t\t\tvar itemToRemove = _context.";
        const char *string22 = ".SingleOrDefault(r => r.ID == id);\n\t\t\tif (itemToRemove != null)\n\t\t\t{\n\t\t\t\t_context.";
        const char *string23 = ".Remove(itemToRemove);\n\t\t\t\t_context.SaveChanges();\n\t\t\t}\n\n\t\t}\n\n\t\tpublic void Update(";
        const char *string24 = " item)\n\t\t{\n\t\t\tvar itemToUpdate = _context.";
        const char *string25 = ".SingleOrDefault(r => r.ID == item.ID);\n\t\t\tif (itemToUpdate != null)\n\t\t\t{\n";
        const char *string26 = "\t\t\t\t_context.SaveChanges();\n\t\t\t}\n\t\t}\n\t}\n}\n";
        
        fprintf(file_ptr,"%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s",
               string1, APICallName, string2, APICallName, string3, tableName, string4, APICallName,
               string5, tableName, string6, tableName, string7, tableName, string8, tableName,
               string9, tableName, string10, tableName, string11, tableName, string12, APICallName,
               string13, tableName, string14, APICallName, string15, APICallName, string16, tableName,
               string17, tableName, string18, tableName, string02, tableName, string03, tableName, string04, tableName, string19, APICallName, string20, APICallName,
               string21);
        
//        , tableName, string22, tableName, string23, tableName, string24, tableName,
//        string25
        
        char *tempString1;
        int i = 0;
        while(i < intIndex) {
            tempString1 = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString1, "\t\t\t\t\tdisp.%s = t.%s;\n", ints[i], ints[i]);
            fprintf(file_ptr, "%s", tempString1);
            free(tempString1);
            i++;
        }
        i = 0;
        while(i < stringIndex) {
            tempString1 = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString1, "\t\t\t\t\tdisp.%s = t.%s;\n", strings[i], strings[i]);
            fprintf(file_ptr, "%s", tempString1);
            free(tempString1);
            i++;
        }
        i = 0;
        while(i < decIndex) {
            tempString1 = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString1, "\t\t\t\t\tdisp.%s = t.%s;\n", decimals[i], decimals[i]);
            fprintf(file_ptr, "%s", tempString1);
            free(tempString1);
            i++;
        }
        fprintf(file_ptr, "%s%s%s%s%s%s%s%s%s", string01, tableName, string22, tableName, string23, tableName, string24, tableName, string25);
        
        char *tempString2;
        i = 0;
        while(i < intIndex) {
            tempString2 = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString2, "\t\t\t\titemToUpdate.%s = item.%s;\n", ints[i], ints[i]);
            fprintf(file_ptr, "%s", tempString2);
            free(tempString2);
            i++;
        }
        i = 0;
        while(i < stringIndex) {
            tempString2 = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString2, "\t\t\t\titemToUpdate.%s = item.%s;\n", strings[i], strings[i]);
            fprintf(file_ptr, "%s", tempString2);
            free(tempString2);
            i++;
        }
        i = 0;
        while(i < decIndex) {
            tempString2 = (char *) malloc(MAX_FILE_STRING * sizeof(char));
            sprintf(tempString2, "\t\t\t\titemToUpdate.%s = item.%s;\n", decimals[i], decimals[i]);
            fprintf(file_ptr, "%s", tempString2);
            free(tempString2);
            i++;
        }
        fprintf(file_ptr, "%s\n", string26);
        
        fclose(file_ptr);
        free(fileName);
    }
}

void writeController() {
    FILE *file_ptr = NULL;
    char *fileName = (char *) malloc(MAX_FILE_STRING * sizeof(char));
    strcpy(fileName, "Ario.API/Controllers/");
    strcat(fileName, APICallName);
    strcat(fileName, "Controller.cs");
    
    fileAccess(fileName, &file_ptr);
    
    if(file_ptr != NULL) {
        const char *string1 = "using Ario.API.Models;\nusing Ario.API.Repositories;\nusing Microsoft.AspNetCore.Mvc;\nusing System.Collections.Generic;\nusing Ario.API.Models.DisplayModels;\n\nnamespace Ario.API.Controllers\n{\n\t[Route(\"api/[controller]\")]\n\tpublic class ";
        const char *string2 = "Controller : Controller\n\t{\n\t\tpublic I";
        const char *string3 = "Repository ";
        const char *string4 = "Repo { get; set; }\n\n\t\tpublic ";
        const char *string5 = "Controller(I";
        const char *string6 = "Repository _repo)\n\t\t{\n\t\t\t";
        const char *string7 = "Repo = _repo;\n\t\t}\n\n\t\t[HttpGet(\"all\")]\n\t\tpublic IEnumerable<";
        const char *string8 = "> GetAll()\n\t\t{\n\t\t\treturn ";
        const char *string9 = "Repo.GetAll();\n\t\t}\n\n\t\t[HttpGet]\n\t\tpublic IEnumerable<";
        const char *string10 = "Display> GetAll([FromQuery] ";
        const char *string11 = " item)\n\t\t{\n\t\t\treturn ";
        const char *string12 = "Repo.GetAll(item);\n\t\t}\n\n\t\t[HttpGet(\"{id}\", Name = \"";
        const char *string13 = "\")]\n\t\tpublic IActionResult GetById(int id)\n\t\t{\n\t\t\tvar item = ";
        const char *string14 = "Repo.Find(id);\n\t\t\tif (item == null)\n\t\t\t{\n\t\t\t\treturn NotFound();\n\t\t\t}\n\t\t\treturn new ObjectResult(item);\n\t\t}\n\n\t\t[HttpPost]\n\t\tpublic IActionResult Create([FromBody] ";
        const char *string15 = " item)\n\t\t{\n\t\t\tif (item == null)\n\t\t\t{\n\t\t\t\treturn BadRequest();\n\t\t\t}\n\t\t\t";
        const char *string16 = "Repo.Add(item);\n\t\t\treturn CreatedAtRoute(\"";
        const char *string17 = "\", new { Controller = \"";
        const char *string18 = "\", id = item.ID }, item);\n\t\t}\n\n\t\t[HttpPut(\"{id}\")]\n\t\tpublic IActionResult Update(int id, [FromBody] ";
        const char *string19 = " item)\n\t\t{\n\t\t\tif (item == null)\n\t\t\t{\n\t\t\t\treturn BadRequest();\n\t\t\t}\n\t\t\tvar contactObj = ";
        const char *string20 = "Repo.Find(id);\n\t\t\tif (contactObj == null)\n\t\t\t{\n\t\t\t\treturn NotFound();\n\t\t\t}\n\t\t\t";
        const char *string21 = "Repo.Update(item);\n\t\t\treturn new NoContentResult();\n\t\t}\n\n\t\t[HttpDelete(\"{id}\")]\n\t\tpublic void Delete(int id)\n\t\t{\n\t\t\t";
        const char *string22 = "Repo.Remove(id);\n\t\t}\n\t}\n}\n";

        fprintf(file_ptr,"%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s\n", string1, APICallName, string2, APICallName, string3, tableName, string4, APICallName,
               string5, APICallName, string6, tableName, string7, tableName, string8, tableName,
               string9, APICallName, string10, tableName, string11, tableName, string12, APICallName,
               string13, tableName, string14, tableName, string15, tableName, string16, APICallName,
               string17, tableName, string18, tableName, string19, tableName, string20, tableName,
               string21, tableName, string22);
        
        fclose(file_ptr);
        free(fileName);
    }
}

void getCommandLine(int argc, char **argv) {
    
    int c;
    extern char *optarg;
    
    while((c = getopt(argc, argv, "a:t:s:i:d:")) != -1) {
        switch (c) {
            case 't':
                tableName = optarg;
                break;
            case 'a':
                APICallName = optarg;
                break;
            case 's':
                if(stringIndex < MAX_STRINGS) {
                    strings[stringIndex] = (char *) malloc(MAX_NAME_STRING * sizeof(char));
                    strings[stringIndex] = optarg;
                    stringIndex++;
                }
                break;
            case 'i':
                if(intIndex < MAX_INTS) {
                    ints[intIndex] = (char *) malloc(MAX_NAME_STRING * sizeof(char));
                    ints[intIndex] = optarg;
                    intIndex++;
                }
                break;
            case 'd':
                if(decIndex < MAX_DECIMALS) {
                    decimals[decIndex] = (char *) malloc(MAX_NAME_STRING * sizeof(char));
                    decimals[decIndex] = optarg;
                    decIndex++;
                }
                break;
            default:
                printf("Generator command line options:\n");
                printf(" -t \t enter name of the table (required)\n");
                printf(" -a \t enter name of API call (default is table name)\n");
                printf(" -s \t enter string field for model\n");
                printf(" -i \t enter integer field for model\n");
                printf(" -d \t enter decimal field for model\n");
                exit(1);
        }
    }
}

void fileAccess(char *fileName, FILE **file_ptr) {
    if( access( fileName, F_OK ) == -1 ) {
        *file_ptr = fopen(fileName, "w");
    } else {
        char confirm;
        printf("%s already exists. Enter \"y\" to overwrite.\n", fileName);
        confirm = getchar();
        if(getchar() != '\n') {
            printf("ERROR: Invalid input. Only input a single character.\n");
            exit(1);
        }
        
        if(confirm == 'y') {
            printf("Overwriting...\n");
            *file_ptr = fopen(fileName, "w");
        } else {
            printf("%s was unchanged...\n", fileName);
            return;
        }
    }
}

void cleanup() {
    
    int i = 0;
    while(strings[i] != '\0' && i < MAX_STRINGS) {
        free(strings[i]);
        i++;
    }
    i = 0;
    while(ints[i] != '\0' && i < MAX_INTS) {
        free(ints[i]);
        i++;
    }
    i = 0;
    while(decimals[i] != '\0' && i < MAX_DECIMALS) {
        free(decimals[i]);
        i++;
    }
    
    free(strings);
    free(ints);
    free(decimals);
//    free(tableName);
//    free(APICallName);
}
