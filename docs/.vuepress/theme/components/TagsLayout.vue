<template>
    <div class="content__default">

        <h1>Tags</h1>

        <div class="blog-tags">
            <BlogTag
                    v-for="tag in $frontmatterKey.list"
                    :key="tag.name"
                    :tag="tag"
            />
        </div>
    </div>
</template>

<script>
    import BlogTag from "@theme/components/BlogTag";

    export default {
        components: {BlogTag}
    }
</script>

<style lang="stylus">
    .blog-tags
        width 66%

    @media screen and (max-width: 1000px)
        .blog-tags
            width 90%
</style>
