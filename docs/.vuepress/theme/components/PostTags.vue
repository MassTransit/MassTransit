<template>
    <div>
        <ul v-for="tag in tags" class="tag-list">
            <li class="tag-list__item">
                <router-link class="tag-list__btn" :to="tagPath(tag)">{{ tag }}</router-link>
            </li>
        </ul>
    </div>
</template>

<script>
    export default {
        props: {
            tags: {
                type: Array,
                required: true
            }
        },
        methods: {
            tagPath(tag) {
                return this.$tag._metaMap[tag].path;
            }
        }
    }
</script>

<style scoped lang="stylus">
    .tag-list
        list-style: none
        padding-left: 0
        display: flex
        margin-bottom: 25px

    .tag-list__item
        margin-left: 10px

        &:first-child
            margin-left: 0

    .tag-list__btn
        border-radius: 4px
        color $textColor
        border 1px solid $borderColor
        padding: 5px
        font-size: 0.9rem
        transition: box-shadow 0.2s ease-in

        &:hover
            box-shadow 0 0 5px $accentColor
</style>