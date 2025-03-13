export default defineAppConfig({
    github: {
        owner: 'MassTransit',
        repo: 'MassTransit',
        branch: 'develop'
    },
    docus: {
        title: 'MassTransit',
        description: 'An open-source distributed application framework for .NET',
        image: 'https://masstransit.io/mt-logo-color.png',
        url: 'https://masstransit.io',
        socials: {
            github: 'MassTransit/MassTransit'
        },
        aside: {
            level: 1,
            exclude: []
        },
        header: {
            showLinkIcon: true,
            exclude: []
        },
        footer: {
            credits: {
                text: 'Copyright 2025 Chris Patterson',
                href: 'https://masstransit.io',
                icon: 'IconMassTransit'
            },
            icons: [],
        },
    }
})
