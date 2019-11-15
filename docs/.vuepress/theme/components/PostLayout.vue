<template>
    <div class="content__default">
        <h1 v-if="$title">{{ $title }}</h1>
        Written <span v-if="$frontmatter.author"> by {{ $frontmatter.author }} </span>
        <time v-if="$frontmatter.date">on {{ resolvePostDate }}</time>

        <PostTags :tags="$frontmatter.tag" />

        <Content/>
    </div>
</template>

<script>
    import PostTags from "./PostTags";

    export default {
        components: {PostTags},

        computed: {
            resolvePostDate() {
                return new Date(this.$frontmatter.date).toDateString();
            }
        },

        methods: {}
    }
</script>

<style scoped>
</style>
