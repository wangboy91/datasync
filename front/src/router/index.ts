import { createRouter, createWebHistory } from "vue-router";
import Dashboard from "../pages/Dashboard.vue";
import Sources from "../pages/Sources.vue";
import Targets from "../pages/Targets.vue";
import Schema from "../pages/Schema.vue";
import RulesList from "../pages/RulesList.vue";
import Rules from "../pages/Rules.vue";
import Execute from "../pages/Execute.vue";
import Jobs from "../pages/Jobs.vue";
import Settings from "../pages/Settings.vue";

const routes = [
  { path: "/", component: Dashboard },
  { path: "/sources", component: Sources },
  { path: "/targets", component: Targets },
  { path: "/schema", component: Schema },
  { path: "/rules-list", component: RulesList },
  { path: "/rules", component: Rules },
  { path: "/execute", component: Execute },
  { path: "/jobs", component: Jobs },
  { path: "/settings", component: Settings }
];

export default createRouter({
  history: createWebHistory(),
  routes
});

