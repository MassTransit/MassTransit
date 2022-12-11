export default defineNuxtConfig({
    extends: '@nuxt-themes/docus',
    css: ['~/assets/css/main.css'],
    colorMode: {
        preference: 'dark'
    },
    content: {
        documentDriven: true,
        highlight: {
            theme: {
                dark: 'material-darker',
                default: 'material-lighter'
            },
            preload: ['json', 'shell', 'markdown', 'yaml', 'bash', 'csharp']
        },
        navigation: {
            fields: ['icon', 'titleTemplate', 'aside']
        }
    },
    postcss: {
        plugins: {
            'tailwindcss/nesting': {},
            tailwindcss: {},
            autoprefixer: {},
        },
    },
    runtimeConfig: {
        public: {
            algolia: {
                applicationId: process.env.ALGOLIA_APP_ID,
                apiKey: process.env.ALGOLIA_API_KEY,
                langAttribute: 'lang',
                docSearch: {
                    indexName: 'masstransit_io'
                }
            }
        }
    }
})
