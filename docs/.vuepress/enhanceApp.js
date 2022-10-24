export default ({ router }) => {
  router.addRoutes([
    { path: '/support', redirect: '/learn/support.html' }
  ]);

  router.beforeResolve((to, _from, next) => {
    const browserWindow = typeof window !== "undefined" ? window : null;
    if (browserWindow && to.matched.length && to.matched[0].path !== '*' && to.redirectedFrom) {
      browserWindow.location.href = to.fullPath;
    } else {
      next();
    }
  });
}