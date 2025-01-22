export default defineAppConfig({
    github: {
        owner: 'MassTransit',
        repo: 'MassTransit',
        branch: 'develop'
    },
    docus: {
        title: 'MassTransit',
        description: 'An open-source distributed application framework for .NET',
        image: 'https://raw.githubusercontent.com/phatboyg/MassTransit/docus/docus/public/landing-image.png',
        url: 'https://masstransit-project.com',
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
                text: 'Copyright 2024 Chris Patterson',
                href: 'https://masstransit.io',
                icon: 'IconMassTransit'
            },
            icons: [],
        },
    }
})
