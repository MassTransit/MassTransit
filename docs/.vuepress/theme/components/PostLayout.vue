<template>
    <div class="content__default">
        <h1 v-if="$title">{{ $title }}</h1>
        Written <span v-if="$frontmatter.author"> by {{ $frontmatter.author }} </span>
        <time v-if="$frontmatter.date">on {{ resolvePostDate }}</time>

        <ul v-for="tag in $frontmatter.tag" class="tag-list">
            <li class="tag-list__item">
                <span class="tag-list__btn">{{ tag }}</span>
            </li>
        </ul>

        <Content/>
    </div>
</template>

<script>
    export default {
        computed: {
            resolvePostDate() {
                return new Date(this.$frontmatter.date).toDateString();
            }
        },

        methods: {}
    }
</script>

<style scoped>
    .tag-list {
        list-style: none;
        padding-left: 0;
        display: flex;
        margin-bottom: 25px;
    }

    .tag-list__item {
        margin-left: 10px;
    }

    .tag-list__item:first-child {
        margin-left: 0;
    }

    .tag-list__btn {
        border-radius: 4px;
        border: 1px solid #32c8cf;
        padding: 5px;
        font-size: 0.9rem;
        background-color: #fff;
    }
</style>