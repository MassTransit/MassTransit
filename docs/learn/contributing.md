# Contributing

MassTransit is an open-source project and it means that it relies not only on the core group of people who created
it, but also on **you**!

If you have a brilliant idea how to make MassTransit even better - get in touch with others using GitHub issues,
Google group or Gitter, and if you are ready to make a code contribution, do the following:

* Make your fork of the MassTransit repository
* Clone the fork to your machine
* HACK!
* Submit a pull request

::: tip NOTE
I use JetBrains Rider on a Mac for almost all development, including MassTransit. Which also means that I don't use Visual Studio (or VS Code). The solution and project files are fully compatible, and can be modified on either operating system using any of the supported development tools. The Resharper code formatting tools are used to ensure a consistent source code structure.
:::

### Documentation

MassTransit documentation is hosted on GitHub Pages and uses [VuePress](https://vuepress.vuejs.org/). In order to be able to build the documentation locally, you need to have Node on your machine.

If you want to contribute to this documentation, clone MassTransit, and type `npm run docs:dev` to launch the server. The server automatically rebuilds and pushes updates to the browser as changes are saved, so it's super easy. I might like it too much, at this point, compared to everything I've used previously.

When all these steps are done, you will be able to see the site [locally](http://localhost:8080/).

You can also edit pages in place using GitHub.

[1]: https://toolchain.gitbook.com/syntax/markdown.html