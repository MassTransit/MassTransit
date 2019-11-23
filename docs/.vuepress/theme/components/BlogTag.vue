<template>
    <router-link class="blog-tag" :to="tag.path">
        {{ tag.name }} ({{ tag.pages.length }})
    </router-link>
</template>

<script>
    export default {
        props: ['tag'],
    }
</script>

<style lang="stylus">
    .blog-tag
        display inline-flex
        align-items center
        height 45px
        word-break break-word
        font-size 20px
        margin-right 20px
        margin-bottom 20px
        padding 0 15px
        border-radius 5px
        font-weight: 300
        text-align left
        box-sizing border-box
        color $textColor
        border 1px solid $borderColor
        text-decoration none
        transition: all 0.2s ease-in
        &:hover
            border 1px solid $accentColor
            box-shadow 0 0 5px $accentColor

    @media screen and (max-width: 768px)
        .blog-tag
            font-size 14px
            padding 3px 10px
            margin-right 10px
            margin-bottom 10px
</style>
