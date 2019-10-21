# Contributing

MassTransit is an open-source project and it means that it relies not only on the core group of people who created
it, but also on **you**!

If you have a brilliant idea how to make MassTransit even better - get in touch with others using GitHub issues,
Google group or Gitter, and if you are ready to make a code contribution, do the following:

* Make your fork of the MassTransit repository
* Clone the fork to your machine
* HACK!
* Submit a pull request

## Documentation

MassTransit documentation is hosted on GitHub Pages and uses the [Gitbook](https://toolchain.gitbook.com/). In order to be able to build the documentation locally, you need to have Node.js on your machine.

If you want to contribute to this documentation, you need to:

* Install Gitbook CLI
    `$ npm install -g gitbook-cli`

* Install npm v3 to avoid some issues of npm v2
    `$ npm install -g npm`

* Go to the MassTransit local clone directory and run
    * macOS, Linux
        `$ npm install`
    * Windows
        `$ npm install --no-optional`

* Install Gitbook files
    `$ gitbook install`

* Run Gitbook site
    `$ gitbook serve`

When all these steps are done, you will be able to see the site on http://localhost:4000. Each time you make changes to the documentation files ([Github Markdown][1] files located in the `docs`folder), Gitbook will rebuild the site and refresh you browser.

After you have done all the changes, commit, push and submit a pull request as usual.


[1]: https://toolchain.gitbook.com/syntax/markdown.html