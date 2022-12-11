import VueGtag from 'vue-gtag-next'
export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.use(VueGtag, {
    property: {
      id: '348762328'
    }
  })
})