<script setup lang="ts">
import {groupContent} from "~/lib/content";

type Props = {
    path: string
}
const props = defineProps<Props>()
const parts = props.path.split('/')
const navigationTree = await queryContent(parts[0], ...parts.slice(1))
    // .where({_partial: {$eq: false}})
    .without(['excerpt', 'body'])
    .find();
const grouped = groupContent(navigationTree)
</script>

<template>

    <div class="flex flex-col space-y-8">
        <div class="pl-2 grid grid-cols-2 gap-1">
            <!-- if title === 'overview' do basis-full -->
            <div v-for="page in grouped.pages" class="border border-slate-100 rounded px-3 py-4"
                 :class="{'col-span-2': page.title === 'Overview'}"
            >
                <NuxtLink :to="page._path">
                {{page.title}}
                </NuxtLink>
            </div>
        </div>

        <div v-for="child in grouped.children" class="pl-2">
            <h1 class="py-2">{{child.attributes.title}}</h1>
            <p>
                {{child.attributes.description}}
            </p>
            <div class="pl-2 grid grid-cols-2 gap-1">
                <!-- if title === 'overview' do basis-full -->
                <div v-for="page in child.pages" class="border border-slate-100 rounded px-3 py-4"
                :class="{'col-span-2': page.title === 'Overview'}"
                >
                    {{page.title}}
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>

</style>
