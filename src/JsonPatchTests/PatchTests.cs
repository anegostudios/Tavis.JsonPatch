using System.IO;
using System.Text.Json.Nodes;
using JsonPatch.Adaptors;
using JsonPatch.Operations;
using JsonPatch.Operations.Abstractions;
using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class PatchTests
    {
        /// <summary>
        /// Creates the empty patch.
        /// </summary>
        [Fact]
        public void CreateEmptyPatch()
        {

            var sample = GetSample2();
            var sampletext = sample.ToString();

            var patchDocument = new PatchDocument();
            patchDocument.ApplyTo(new JsonNetTargetAdapter(sample));

            Assert.Equal(sampletext,sample.ToString());
        }


        [Fact]
        public void LoadPatch1()
        {
            var patchDoc =
                PatchDocument.Load(this.GetType()
                    .Assembly.GetManifestResourceStream(this.GetType(), "Samples.LoadTest1.json"));

            Assert.NotNull(patchDoc);
            Assert.Equal(6,patchDoc.Operations.Count);
        }


        [Fact]
        public void TestExample1()
        {
          var targetDoc = JsonNode.Parse("""{ "foo": "bar"}""")!.AsObject();
          var patchDoc = PatchDocument.Parse("""
                                             [
                                                 { "op": "add", "path": "/baz", "value": "qux" }
                                             ]
                                             """);
            patchDoc.ApplyTo(targetDoc);
            var actual = JsonNode.Parse("""
                                         {
                                           "foo": "bar",
                                           "baz": "qux"
                                         }
                                        """);
            Assert.Equal(actual.ToJsonString(), targetDoc.ToJsonString());
        }


  

        [Fact]
        public void SerializePatchDocument()
        {
            var patchDoc = new PatchDocument( new Operation[]
            {
             new TestOperation() {Path = new JsonPointer("/a/b/c"), Value = JsonValue.Create("foo")}, 
             new RemoveOperation() {Path = new JsonPointer("/a/b/c") }, 
             new AddMergeOperation() {Path = new JsonPointer("/a/b/c"), Value = new JsonArray("foo", "bar")}, 
             new ReplaceOperation() {Path = new JsonPointer("/a/b/c"), Value = JsonValue.Create(42)}, 
             new MoveOperation() {FromPath = new JsonPointer("/a/b/c"), Path = new JsonPointer("/a/b/d") }, 
             new CopyOperation() {FromPath = new JsonPointer("/a/b/d"), Path = new JsonPointer("/a/b/e") }, 
            });

            var outputstream = patchDoc.ToStream();
            var output = new StreamReader(outputstream).ReadToEnd();

            var jOutput = JsonNode.Parse(output);

            var jExpected = JsonNode.Parse(new StreamReader(this.GetType()
                .Assembly.GetManifestResourceStream(this.GetType(), "Samples.LoadTest1.json")).ReadToEnd());
            //Assert.True(JsonNode.DeepEquals(jExpected,jOutput));
        }



        public static JsonObject GetSample2()
        {
            return JsonNode.Parse("""
                                  {
                                    "books": [
                                      {
                                        "title" : "The Great Gatsby",
                                        "author" : "F. Scott Fitzgerald"
                                      },
                                      {
                                        "title" : "The Grapes of Wrath",
                                        "author" : "John Steinbeck"
                                      }
                                    ]
                                  }
                                  """)
              ?.AsObject();
        }

        public static JsonObject GetSample3()
        {
            return JsonNode.Parse("""
{
  "code": "clayshingleblock",
  "attributes": {
    "handbook": {
      "groupBy": [
        "clayshingleblock-*"
      ]
    }
  },
  "variantgroups": [
    {
      "code": "variant",
      "states": [
        "blue",
        "brown",
        "fire",
        "red",
        "black",
        "blurple",
        "darkgrey",
        "green",
        "light",
        "malachite",
        "orange",
        "pink",
        "purple",
        "yellow"
      ]
    }
  ],
  "creativeinventory": {
    "general": [
      "clayshingleblock-*"
    ],
    "construction": [
      "clayshingleblock-*"
    ],
    "ceramics": [
      "clayshingleblock-*"
    ]
  },
  "shape": {
    "base": "block/basic/cube"
  },
  "drawtype": "Cube",
  "sidesolid": {
    "all": true
  },
  "sideopaque": {
    "all": true
  },
  "blockmaterial": "Ceramic",
  "replaceable": 600,
  "resistance": 2.5,
  "lightAbsorption": 99,
  "textures": {
    "all": {
      "base": "block/clay/shingles/{variant}"
    }
  },
  "sounds": {
    "walk": "walk/stone"
  },
  "heldTpIdleAnimation": "holdbothhandslarge",
  "heldTpUseAnimation": "twohandplaceblock",
  "tpHandTransform": {
    "translation": {
      "x": -1.2,
      "y": -1.1,
      "z": -0.8
    },
    "rotation": {
      "x": -2,
      "y": 25,
      "z": -78
    },
    "scale": 0.37
  }
}
""")!.AsObject();
        }

        public static JsonObject GetSample4()
        {
            return JsonNode.Parse("""
{
  "code": "trough",
  "class": "BlockTroughDoubleBlock",
  "entityClassByType": {
    "*-head-*": "Trough"
  },
  "attributes": {
    "handbook": {
      "groupBy": [
        "trough-*-large-*"
      ]
    },
    "contentConfig": [
      {
        "code": "flax",
        "content": {
          "type": "item",
          "code": "grain-flax"
        },
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-acceptable-pigs-sheep",
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "textureCode": "contents-flax"
      },
      {
        "code": "rice",
        "content": {
          "type": "item",
          "code": "grain-rice"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "foodFor": [],
        "foodForDesc": "trough-noanimal",
        "textureCode": "contents-rice"
      },
      {
        "code": "rye",
        "content": {
          "type": "item",
          "code": "grain-rye"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-treat-pigs-sheep",
        "textureCode": "contents-rye"
      },
      {
        "code": "spelt",
        "content": {
          "type": "item",
          "code": "grain-spelt"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-acceptable-pigs-sheep",
        "textureCode": "contents-spelt"
      },
      {
        "code": "amaranth",
        "content": {
          "type": "item",
          "code": "grain-amaranth"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-acceptable-pigs-sheep",
        "textureCode": "contents-amaranth"
      },
      {
        "code": "sunflower",
        "content": {
          "type": "item",
          "code": "grain-sunflower"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-acceptable-pigs-sheep",
        "textureCode": "contents-sunflower"
      },
      {
        "code": "bellpepper",
        "content": {
          "type": "item",
          "code": "vegetable-bellpepper"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/bellpepperfill1",
          "block/wood/trough/large/bellpepperfill1",
          "block/wood/trough/large/bellpepperfill2",
          "block/wood/trough/large/bellpepperfill2",
          "block/wood/trough/large/bellpepperfill3",
          "block/wood/trough/large/bellpepperfill3",
          "block/wood/trough/large/bellpepperfill4",
          "block/wood/trough/large/bellpepperfill4"
        ],
        "foodFor": [
          "pig-*"
        ],
        "foodForDesc": "trough-treat-pigs"
      },
      {
        "code": "cabbage",
        "content": {
          "type": "item",
          "code": "vegetable-cabbage"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/cabbagefill1",
          "block/wood/trough/large/cabbagefill1",
          "block/wood/trough/large/cabbagefill2",
          "block/wood/trough/large/cabbagefill2",
          "block/wood/trough/large/cabbagefill3",
          "block/wood/trough/large/cabbagefill3",
          "block/wood/trough/large/cabbagefill4",
          "block/wood/trough/large/cabbagefill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-treat-pigs-sheep"
      },
      {
        "code": "carrot",
        "content": {
          "type": "item",
          "code": "vegetable-carrot"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/carrotfill1",
          "block/wood/trough/large/carrotfill1",
          "block/wood/trough/large/carrotfill2",
          "block/wood/trough/large/carrotfill2",
          "block/wood/trough/large/carrotfill3",
          "block/wood/trough/large/carrotfill3",
          "block/wood/trough/large/carrotfill4",
          "block/wood/trough/large/carrotfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-treat-pigs-sheep"
      },
      {
        "code": "cassava",
        "content": {
          "type": "item",
          "code": "vegetable-cassava"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/cassavafill1",
          "block/wood/trough/large/cassavafill1",
          "block/wood/trough/large/cassavafill2",
          "block/wood/trough/large/cassavafill2",
          "block/wood/trough/large/cassavafill3",
          "block/wood/trough/large/cassavafill3",
          "block/wood/trough/large/cassavafill4",
          "block/wood/trough/large/cassavafill4"
        ],
        "foodFor": [
          "pig-*"
        ],
        "foodForDesc": "trough-acceptable-pigs"
      },
      {
        "code": "onion",
        "content": {
          "type": "item",
          "code": "vegetable-onion"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/onionfill1",
          "block/wood/trough/large/onionfill1",
          "block/wood/trough/large/onionfill2",
          "block/wood/trough/large/onionfill2",
          "block/wood/trough/large/onionfill3",
          "block/wood/trough/large/onionfill3",
          "block/wood/trough/large/onionfill4",
          "block/wood/trough/large/onionfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-treat-pigs-sheep"
      },
      {
        "code": "parsnip",
        "content": {
          "type": "item",
          "code": "vegetable-parsnip"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/parsnipfill1",
          "block/wood/trough/large/parsnipfill1",
          "block/wood/trough/large/parsnipfill2",
          "block/wood/trough/large/parsnipfill2",
          "block/wood/trough/large/parsnipfill3",
          "block/wood/trough/large/parsnipfill3",
          "block/wood/trough/large/parsnipfill4",
          "block/wood/trough/large/parsnipfill4"
        ],
        "foodFor": [],
        "foodForDesc": "trough-noanimal"
      },
      {
        "code": "pumpkin",
        "content": {
          "type": "block",
          "code": "pumpkin-fruit-4"
        },
        "quantityPerFillLevel": 1,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/pumpkinfill1",
          "block/wood/trough/large/pumpkinfill1",
          "block/wood/trough/large/pumpkinfill2",
          "block/wood/trough/large/pumpkinfill2",
          "block/wood/trough/large/pumpkinfill3",
          "block/wood/trough/large/pumpkinfill3",
          "block/wood/trough/large/pumpkinfill4",
          "block/wood/trough/large/pumpkinfill4"
        ],
        "foodFor": [
          "pig-*"
        ],
        "foodForDesc": "trough-treat-pigs"
      },
      {
        "code": "peanut",
        "content": {
          "type": "item",
          "code": "legume-peanut"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/legumefill1",
          "block/wood/trough/large/legumefill1",
          "block/wood/trough/large/legumefill2",
          "block/wood/trough/large/legumefill2",
          "block/wood/trough/large/legumefill3",
          "block/wood/trough/large/legumefill3",
          "block/wood/trough/large/legumefill4",
          "block/wood/trough/large/legumefill4"
        ],
        "foodFor": [
          "pig-*"
        ],
        "foodForDesc": "trough-treat-pigs",
        "textureCode": "contents-peanut"
      },
      {
        "code": "soybean",
        "content": {
          "type": "item",
          "code": "legume-soybean"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/legumefill1",
          "block/wood/trough/large/legumefill1",
          "block/wood/trough/large/legumefill2",
          "block/wood/trough/large/legumefill2",
          "block/wood/trough/large/legumefill3",
          "block/wood/trough/large/legumefill3",
          "block/wood/trough/large/legumefill4",
          "block/wood/trough/large/legumefill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-treat-pigs-sheep",
        "textureCode": "contents-soybean"
      },
      {
        "code": "turnip",
        "content": {
          "type": "item",
          "code": "vegetable-turnip"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/turnipfill1",
          "block/wood/trough/large/turnipfill1",
          "block/wood/trough/large/turnipfill2",
          "block/wood/trough/large/turnipfill2",
          "block/wood/trough/large/turnipfill3",
          "block/wood/trough/large/turnipfill3",
          "block/wood/trough/large/turnipfill4",
          "block/wood/trough/large/turnipfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-acceptable-pigs-sheep"
      },
      {
        "code": "drygrass",
        "content": {
          "type": "item",
          "code": "drygrass"
        },
        "quantityPerFillLevel": 8,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/hayfill1",
          "block/wood/trough/large/hayfill1",
          "block/wood/trough/large/hayfill2",
          "block/wood/trough/large/hayfill2",
          "block/wood/trough/large/hayfill3",
          "block/wood/trough/large/hayfill3",
          "block/wood/trough/large/hayfill4",
          "block/wood/trough/large/hayfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-staple-sheep-acceptable-pigs"
      },
      {
        "code": "hay",
        "content": {
          "type": "block",
          "code": "hay-normal"
        },
        "quantityPerFillLevel": 1,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/hayfill1",
          "block/wood/trough/large/hayfill1",
          "block/wood/trough/large/hayfill2",
          "block/wood/trough/large/hayfill2",
          "block/wood/trough/large/hayfill3",
          "block/wood/trough/large/hayfill3",
          "block/wood/trough/large/hayfill4",
          "block/wood/trough/large/hayfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-staple-sheep-acceptable-pigs"
      },
      {
        "code": "fruitmash",
        "content": {
          "type": "item",
          "code": "pressedmash-apple"
        },
        "quantityPerFillLevel": 2,
        "maxFillLevels": 8,
        "shapesPerFillLevel": [
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill1",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill2",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill3",
          "block/wood/trough/large/grainfill4",
          "block/wood/trough/large/grainfill4"
        ],
        "foodFor": [
          "pig-*",
          "sheep-*"
        ],
        "foodForDesc": "trough-acceptable-pigs-sheep",
        "textureCode": "contents-fruitmash"
      }
    ]
  },
  "sidesolid": {
    "all": false
  },
  "sideopaque": {
    "all": false
  },
  "heldTpIdleAnimation": "holdunderarm",
  "variantgroups": [
    {
      "code": "material",
      "states": [
        "genericwood"
      ]
    },
    {
      "code": "part",
      "states": [
        "large-head",
        "large-feet"
      ]
    },
    {
      "code": "side",
      "loadFromProperties": "abstract/horizontalorientation"
    }
  ],
  "creativeinventory": {
    "general": [
      "*-head-north"
    ],
    "decorative": [
      "*-head-north"
    ]
  },
  "shapeinventory": {
    "base": "block/wood/trough/large/inventory"
  },
  "shapebytype": {
    "*-head-north": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 90
    },
    "*-head-east": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 0
    },
    "*-head-south": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 270
    },
    "*-head-west": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 180
    },
    "*-feet-north": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 270
    },
    "*-feet-east": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 180
    },
    "*-feet-south": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 90
    },
    "*-feet-west": {
      "base": "block/wood/trough/large/empty",
      "rotateY": 0
    }
  },
  "textures": {
    "contents-flax": {
      "base": "block/wood/trough/large/flax"
    },
    "contents-rice": {
      "base": "block/wood/trough/large/rice"
    },
    "contents-rye": {
      "base": "block/wood/trough/large/rye"
    },
    "contents-spelt": {
      "base": "block/wood/trough/large/spelt"
    },
    "contents-amaranth": {
      "base": "block/wood/trough/large/amaranth"
    },
    "contents-sunflower": {
      "base": "block/wood/trough/large/sunflower"
    },
    "contents-aubergine": {
      "base": "block/wood/trough/large/aubergine"
    },
    "bellpepper": {
      "base": "block/wood/trough/large/bellpepper"
    },
    "cabbage": {
      "base": "block/wood/trough/large/cabbage"
    },
    "carrot": {
      "base": "block/wood/trough/large/carrot"
    },
    "cassava": {
      "base": "block/wood/trough/large/cassava"
    },
    "onion": {
      "base": "block/wood/trough/large/onion"
    },
    "oniontop": {
      "base": "block/plant/crop/onion/e2"
    },
    "parsnip": {
      "base": "block/wood/trough/large/parsnip"
    },
    "contents-peanut": {
      "base": "block/wood/trough/large/peanut"
    },
    "pumpkin": {
      "base": "block/wood/trough/large/pumpkin"
    },
    "contents-soybean": {
      "base": "block/wood/trough/large/soybean"
    },
    "tomato": {
      "base": "block/wood/trough/large/tomato"
    },
    "turnip": {
      "base": "block/wood/trough/large/turnip"
    },
    "turniptop": {
      "base": "block/plant/crop/turnip/e2"
    },
    "singlehay": {
      "base": "block/wood/trough/large/singlehay"
    },
    "singlehay2": {
      "base": "block/wood/trough/large/singlehay2"
    },
    "normal-side": {
      "base": "block/hay/normal-side"
    },
    "normal-top": {
      "base": "block/hay/normal-top"
    },
    "contents-fruitmash": {
      "base": "block/wood/trough/large/fruitmash"
    },
    "rotoverlay": {
      "base": "item/resource/rot/rotoverlay"
    },
    "rot": {
      "base": "block/wood/trough/large/rot"
    }
  },
  "blockmaterial": "Wood",
  "replaceable": 550,
  "resistance": 2,
  "lightAbsorption": 0,
  "combustibleProps": {
    "burnTemperature": 600,
    "burnDuration": 40
  },
  "guiTransform": {
    "translation": {
      "x": -1,
      "y": 2,
      "z": 0
    },
    "origin": {
      "x": 1,
      "y": 0.3,
      "z": 0.5
    },
    "scale": 1.04
  },
  "fpHandTransform": {
    "translation": {
      "x": -0.1,
      "y": -0.62,
      "z": 1.23
    },
    "rotation": {
      "x": 127,
      "y": 140,
      "z": -83
    },
    "scale": 1.6
  },
  "tpHandTransform": {
    "translation": {
      "x": -0.3,
      "y": -0.4,
      "z": -0.4
    },
    "rotation": {
      "x": 85,
      "y": 0,
      "z": 0
    },
    "scale": 0.6
  },
  "groundTransform": {
    "origin": {
      "x": 0.5,
      "y": 0,
      "z": 0.5
    },
    "scale": 2.5
  },
  "selectionbox": {
    "x1": 0.125,
    "y1": 0,
    "z1": 0,
    "x2": 0.875,
    "y2": 0.5625,
    "z2": 0.8125,
    "rotateYByType": {
      "*-feet-north": 180,
      "*-feet-east": 90,
      "*-feet-south": 0,
      "*-feet-west": 270,
      "*-head-north": 0,
      "*-head-east": 270,
      "*-head-south": 180,
      "*-head-west": 90
    }
  },
  "collisionbox": {
    "x1": 0.125,
    "y1": 0,
    "z1": 0,
    "x2": 0.875,
    "y2": 0.5625,
    "z2": 0.8125,
    "rotateYByType": {
      "*-feet-north": 180,
      "*-feet-east": 90,
      "*-feet-south": 0,
      "*-feet-west": 270,
      "*-head-north": 0,
      "*-head-east": 270,
      "*-head-south": 180,
      "*-head-west": 90
    }
  },
  "sounds": {
    "place": "block/planks",
    "hit": "block/planks",
    "break": "block/planks",
    "walk": "walk/wood"
  },
  "materialDensity": 600
}
""")!.AsObject();
        }
    
    
        public static JsonObject GetSample5()
        {
            return JsonNode.Parse("""
[
  {
	"bottom-inside": {
	  "baseByType": {
		"*-genericwood-*": "block/wood/trough/large/bottom-inside",
		"*": "block/wood/debarked/{material}"
	  }
	}
  },
  {
	"legs": {
	  "baseByType": {
		"*-genericwood-*": "block/wood/trough/large/legs",
		"*": "block/wood/debarked/{material}"
	  }
	}
  },
  {
	"wood": {
	  "baseByType": {
		"*-genericwood-*": "block/wood/trough/large/wood",
		"*": "block/wood/debarked/{material}"
	  }
	}
  },
  {
	"outer-ring": {
	  "baseByType": {
		"*-genericwood-*": "block/wood/trough/large/outer-ring",
		"*": "block/wood/debarked/{material}"
	  }
	}
  }
]
""")!.AsObject();
        }
    
    }
}
