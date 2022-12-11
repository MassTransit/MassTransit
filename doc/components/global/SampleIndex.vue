<template>
    <ContentList path="/samples" v-slot="{ list }">
        <div v-for="sample in list" :key="sample._path">
            <h3>{{ sample.title }}</h3>
            <ContentRenderer :value="sample" />
            <div>
                <Icon name="codicon:github-inverted" />
                <strong style="margin-left: 0.5rem">
                    <NuxtLink :to="repoLink" target="_blank" style="margin-right: 0.5rem;">{{sample.repo}}</NuxtLink>
                    <Icon name="heroicons:arrow-top-right-on-square" />
                </strong>
            </div>
        </div>
    </ContentList>
</template>

<script lang="ts" setup>
type Props = {
}

const props = defineProps<Props>()
const samples = await queryContent('samples').find();

</script>

<style>
#sample div p {
    margin: 0;
    margin-top: 0.5rem;
}
</style>
