<template>
    <div
            class="theme-container"
            :class="pageClasses"
            @touchstart="onTouchStart"
            @touchend="onTouchEnd"
    >
        <Navbar
                v-if="shouldShowNavbar"
                @toggle-sidebar="toggleSidebar"
        />

        <div
                class="sidebar-mask"
                @click="toggleSidebar(false)"
        ></div>

        <BlogPage :sidebar-items="[]">
            <template #top>
            </template>
            <template #bottom>
            </template>
        </BlogPage>
    </div>
</template>

<script>
    import Navbar from '@parent-theme/components/Navbar.vue'
    import BlogPage from '@theme/components/BlogPage.vue'
    import Sidebar from '@parent-theme/components/Sidebar.vue'

    export default {
        components: {BlogPage, Sidebar, Navbar},

        data() {
            return {isSidebarOpen: false}
        },

        computed: {
            shouldShowNavbar() {
                const {themeConfig} = this.$site;
                const {frontmatter} = this.$page;

                return frontmatter.navbar === false || themeConfig.navbar === false
                    ? false
                    : (
                        this.$title
                        || themeConfig.logo
                        || themeConfig.repo
                        || themeConfig.nav
                        || this.$themeLocaleConfig.nav
                    );
            },

            pageClasses() {
                const userPageClass = this.$page.frontmatter.pageClass;
                return [
                    {
                        'no-navbar': !this.shouldShowNavbar,
                        'sidebar-open': this.isSidebarOpen,
                        'no-sidebar': true
                    },
                    userPageClass
                ]
            }
        },

        mounted() {
            this.$router.afterEach(() => {
                this.isSidebarOpen = false
            })
        },

        methods: {
            toggleSidebar(to) {
                this.isSidebarOpen = typeof to === 'boolean' ? to : !this.isSidebarOpen;
                this.$emit('toggle-sidebar', this.isSidebarOpen)
            },

            // side swipe
            onTouchStart(e) {
                this.touchStart = {
                    x: e.changedTouches[0].clientX,
                    y: e.changedTouches[0].clientY
                }
            },

            onTouchEnd(e) {
                const dx = e.changedTouches[0].clientX - this.touchStart.x;
                const dy = e.changedTouches[0].clientY - this.touchStart.y;
                if (Math.abs(dx) > Math.abs(dy) && Math.abs(dx) > 40) {
                    this.toggleSidebar(dx > 0 && this.touchStart.x <= 80)
                }
            }
        }
    }
</script>
