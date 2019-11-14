<template>
    <main class="page">
        <slot name="top"/>

        <component :is="layoutComponent" class="theme-default-content"/>
        <PageEdit/>

        <PageNav v-bind="{ sidebarItems }"/>

        <slot name="bottom"/>
    </main>
</template>

<script>
    import PageEdit from '@parent-theme/components/PageEdit.vue';
    import PageNav from '@parent-theme/components/PageNav.vue';
    import TagsLayout from "./TagsLayout";
    import PostsLayout from './PostsLayout.vue';
    import PostLayout from './PostLayout.vue';

    export default {
        components: {PageEdit, PageNav, TagsLayout, PostsLayout, PostLayout},

        props: ['sidebarItems'],

        computed: {
            layoutComponent() {
                let isArray = (p, getArray) => p && getArray(p) && getArray(p).length;

                if (isArray(this.$frontmatterKey, x => x.list))
                    return "TagsLayout";

                if (isArray(this.$pagination, x => x.pages))
                    return "PostsLayout";

                if (this.$page.pid === "post")
                    return "PostLayout";

                return "Content";
            }
        }
    }
</script>

<style lang="stylus">
    @require '../styles/wrapper.styl'
    .page
        padding-bottom 2rem
        display block
</style>
