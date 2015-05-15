#JsCollate
=========

Compresses and collates JavaScript files and updates the HTML file script tags. It is a C# .Net command line app that can be included into your build process.

It's relatively easy to compress and collate multiple JS files but that's only half the battle. You also need to update the script tags in your HTML file to point to the newly created file. You could do it by hand but that's not very efficient.

JsCollate solves this problem by concatenating all of your JS files, compressing them, and updating the HTML file all in one step. Just what you need to easliy release your single page app.

##How to use

There are 2 ways to specify files to collate in your HTML. You can either use data-* attributes on your script elements or use a comment around the script elements. The second method is the newer way to do it and the easiest, so I recommend that.

Simply surround the script tags to be collated with comments. The opening comment has "replace:with(file-name.js)" where file-name.js is the name of the file to collate to. The ending comment has "endreplace".

    <!-- replace:with(app.min.js) -->
    <script src="some-file.js"></script>
    <script src="some-other-file.js"></script>
    <script src="app.js"></script>
    <!-- endreplace -->

In the example above a new file will be created called app.min.js that contains the contents of some-file.js, some-other-file.js and app.js. A new HTML file will also be created that has everything between the comments removed and replaced with a script tag pointing to app.min.js. You may have multiple sections that collate to different files.

The other way to accomplish this is by using data-collate attributes on your script elements.

    <script src="some-file.js" data-collate="app.min.js"></script>
    <script src="some-other-file.js" data-collate="app.min.js"></script>
    <script src="app.js" data-collate="app.min.js"></script>

This requires significantly more typing, that's why I recommend the new way using comments.

##Running JsCollate

JsCollate runs from a Windows command line using the following format.

    JsCollate source dest [/header:text] [/-c]

It takes two required parameters. The first is the path to the source HTML file. The second is the path to the destination folder where the updated HTML file and JavaScript files will be written to. There are two optional parameters. The "/header:" parameter adds some header text to the beginning of the collated JavaScript file(s). This text can be anything, but should be a JavaScript comment. The "/-c" parameter tells JsCollate not to compress the collated JavaScript file(s). This can be helpful for debugging.

For example, this will collate app.html. It will create a new app.html file, and whatever JavaScript files are specified in app.html to the release folder.

    jsCollate src\app.html release /header:"/* Created by JM Gustafson */"

