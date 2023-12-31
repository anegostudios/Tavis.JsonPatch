# Tavis.JsonPatch

This library is (mostly) an implementation of a Json Patch [RFC 6902](http://tools.ietf.org/html/rfc6902).  

Differences from the RFC 6902:
* New `addmerge` op performs a merge if the target is an object
* New `addeach` op to add multiple values to an array


The default example from the specification looks like this,

	[
	     { "op": "test", "path": "/a/b/c", "value": "foo" },
	     { "op": "remove", "path": "/a/b/c" },
	     { "op": "add", "path": "/a/b/c", "value": ["foo", "bar"] },
	     { "op": "replace", "path": "/a/b/c", "value": 42 },
	     { "op": "move", "from": "/a/b/c", "path": "/a/b/d" },
	     { "op": "copy", "from": "/a/b/d", "path": "/a/b/e" }
	]

This library allows you to create this document by doing, 


       var patchDoc = new PatchDocument( new Operation[]
            {
             new TestOperation() {Path = new JsonPointer("/a/b/c"), Value = new JValue("foo")}, 
             new RemoveOperation() {Path = new JsonPointer("/a/b/c") }, 
             new AddOperation() {Path = new JsonPointer("/a/b/c"), Value = new JArray(new JValue("foo"), new JValue("bar"))}, 
             new ReplaceOperation() {Path = new JsonPointer("/a/b/c"), Value = new JValue(42)}, 
             new MoveOperation() {FromPath = new JsonPointer("/a/b/c"), Path = new JsonPointer("/a/b/d") }, 
             new CopyOperation() {FromPath = new JsonPointer("/a/b/d"), Path = new JsonPointer("/a/b/e") }, 
            });

This document can be serialized to the wire format like this,

         var outputstream = patchDoc.ToStream();

You can also read patch documents from the wire representation and apply them to a JSON document.
	
	    var targetDoc = JToken.Parse("{ 'foo': 'bar'}");
        var patchDoc = PatchDocument.Parse(@"[
                      { 'op': 'add', 'path': '/baz', 'value': 'qux' }
                    ]");

        patchDoc.ApplyTo(targetDoc);

        Assert.True(JToken.DeepEquals(JToken.Parse(@"{
                                                         'foo': 'bar',
                                                         'baz': 'qux'
                                                       }"), targetDoc));


You can also apply the Json patch format to other targets by implementing the `IPatchTarget` interface.

The unit tests provide examples of other usages.

This library is a PCL based library and so will work on Windows 8, Windows Phone 7.5, .Net 4.

A nuget package is available [here](http://www.nuget.org/packages/Tavis.JsonPatch).